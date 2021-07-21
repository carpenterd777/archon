package main

import (
	"fmt"
	"image/color"
	"io"
	"strconv"
	"strings"
	"time"

	"fyne.io/fyne/v2"
	"fyne.io/fyne/v2/app"
	"fyne.io/fyne/v2/canvas"
	"fyne.io/fyne/v2/container"
	"fyne.io/fyne/v2/data/binding"
	"fyne.io/fyne/v2/dialog"
	"fyne.io/fyne/v2/theme"
	"fyne.io/fyne/v2/widget"
	"github.com/archon/backend"
	"github.com/archon/gui"
)

const APP_NAME = "Archon"
const DEFAULT_SESSION_NAME = "Untitled Session"
const STARTING_WIDTH = 600
const STARTING_HEIGHT = 400
const MAX_WIN_TITLE_LENGTH = 50

// Handles the rendering for NoteBoxes. Implements the fyne.WidgetRenderer interface.
type MainInterfaceRenderer struct {
	cont *fyne.Container // the container holding all of the items of the main application window
	mi   *MainInterface  // the app window this is rendering
}

// The minimum size of the app window. Necessary to implement the fyne.WidgetRenderer interface.
func (m *MainInterfaceRenderer) MinSize() fyne.Size {
	return m.cont.MinSize()
}

// Position and resize the items within the app window. Necessary to implement the fyne.WidgetRenderer interface.
func (m *MainInterfaceRenderer) Layout(size fyne.Size) {
	m.cont.Resize(size)
	m.cont.Layout.Layout(m.cont.Objects, m.cont.Size())
}

// Apply a theme to all items within the app window. Necessary to implement the fyne.WidgetRenderer interface.
// This function does nothing when called externally, do not call it.
func (m *MainInterfaceRenderer) ApplyTheme() {
	// no-op, allow fyne to manage main interface theme
}

// Triggers when the app window changes or when the theme is changed. Necessary to implement the fyne.WidgetRenderer interface.
func (m *MainInterfaceRenderer) Refresh() {
	canvas.Refresh(m.cont)
	for _, object := range m.cont.Objects {
		canvas.Refresh(object)
	}
}

// Returns the list of objects this renders. Necessary to implement the fyne.WidgetRenderer interface.
func (m *MainInterfaceRenderer) Objects() []fyne.CanvasObject {
	return []fyne.CanvasObject{m.cont}
}

// Called when this rendered is ,no longer needed. Necessary to implement the fyne.WidgetRenderer interface.
func (m *MainInterfaceRenderer) Destroy() {
	// no-op, no resources to close
}

// Represents the main interface of the application window. Implements the widget.Widget interface.
type MainInterface struct {
	widget.BaseWidget
	session     *backend.Session       // The state of this application session
	entry       *gui.EnterEntry        // The entry field
	indicator   *gui.SavingIndicator   // an indicator that flashes when a save is initiated
	infoButton  *widget.Button         // a button containing info for the session
	boundTitle  binding.ExternalString // a binding for the session title
	boundNumber binding.String         // a binding for the session number
	window      fyne.Window            // the window this is rendered in
}

// Creates a renderer for the main window. Necessary to implement the widget.Widget inteface.
func (m *MainInterface) CreateRenderer() fyne.WidgetRenderer {

	list := widget.NewList(
		m.listLength,
		m.listCreateItem,
		m.listUpdateItem,
	)

	toolbar := widget.NewToolbar(
		widget.NewToolbarAction(theme.DocumentSaveIcon(), m.Save),
		widget.NewToolbarAction(theme.FolderOpenIcon(), m.Load),
	)
	m.indicator = gui.NewSavingIndicator()
	m.BindSessionInfo()
	m.infoButton = widget.NewButton(
		m.getInfoButtonText(),
		m.HandleSessionInfoButton,
	)
	cont := container.NewBorder(
		container.NewVBox(container.NewHBox(toolbar, m.infoButton), m.indicator),
		m.entry,
		nil,
		nil,
		list,
	)
	return &MainInterfaceRenderer{
		cont: cont,
		mi:   m,
	}

}

// Returns the length of the data the list widget is displaying.
func (m *MainInterface) listLength() int {
	return len(m.session.Notes)
}

// Creates a template item for the list widget.
func (m *MainInterface) listCreateItem() fyne.CanvasObject {
	return gui.NewNoteBox("", time.Time{})
}

// Sets the actual content of a template item for the list widget when it is displayed.
func (m *MainInterface) listUpdateItem(i widget.ListItemID, o fyne.CanvasObject) {
	o.(*gui.NoteBox).SetContent(m.session.Notes[i].Content)
	o.(*gui.NoteBox).SetTime(m.session.Notes[i].Time)
}

func (m *MainInterface) animateIndicator() {
	disabledToForeground := canvas.NewColorRGBAAnimation(
		theme.DisabledColor(),
		theme.ForegroundColor(),
		canvas.DurationShort,
		func(c color.Color) {
			m.indicator.SetColor(c)
			m.indicator.Refresh()
		})
	disabledToForeground.AutoReverse = true
	disabledToForeground.Start()
}

func (m *MainInterface) Save() {
	// if the user has yet to save their work
	if m.session.Path == "" {
		dialog.ShowFileSave(
			m.save,
			m.window,
		)
	} else { // the user has already saved their work
		err := m.session.Save()
		if err != nil {
			dialog.ShowError(err, m.window)
		}
		m.animateIndicator()
		m.SetWindowTitle()
	}
}

func (m *MainInterface) save(uc fyne.URIWriteCloser, e error) {
	// the user pressed 'cancel'
	if uc == nil {
		return
	}

	reader := strings.NewReader(m.session.ToJSON())
	_, err := reader.WriteTo(uc)

	if e != nil {
		dialog.ShowError(e, m.window)
	}
	if err != nil {
		dialog.ShowError(err, m.window)
	}
	m.session.Path = uc.URI().Path()
	m.SetWindowTitle()
}

func (m *MainInterface) Load() {
	dialog.ShowFileOpen(
		m.load,
		m.window,
	)
}

func (m *MainInterface) load(uc fyne.URIReadCloser, e error) {
	// the user pressed 'cancel'
	if uc == nil {
		return
	}

	builder := new(strings.Builder)
	data := make([]byte, 1024)

	// read data from file into array
	for {
		_, err := uc.Read(data)
		if err == io.EOF {
			break
		}
		if err != nil {
			dialog.ShowError(err, m.window)
		}
	}

	// write data into string builder
	builder.Write(data)

	// construct session from string
	var err error
	m.session, err = backend.FromJSON(builder.String())
	m.entry.SetSession(m.session)
	if err != nil {
		dialog.ShowError(err, m.window)
	}
	m.session.Path = uc.URI().Path()
	m.BindSessionInfo()
	m.SetWindowTitle()
	m.infoButton.SetText(m.getInfoButtonText())
}

// Set the title of the window based on the session title, session number, and path.
func (m *MainInterface) SetWindowTitle() {
	window_title := ""

	if m.session.SessionTitle != "" {
		window_title = m.session.SessionTitle
	}

	if m.session.SessionTitle == "" && m.session.SessionNumber > backend.NO_SESSION_NUMBER {
		window_title = fmt.Sprintf("Session %d %s", m.session.SessionNumber, m.session.Path)
	}

	if len(window_title) > MAX_WIN_TITLE_LENGTH {
		// subtract 3 to account for the max length, subtract 1 because indexing starts at 0
		window_title = window_title[:MAX_WIN_TITLE_LENGTH-3-1] + "..."
	}
	window_title = fmt.Sprintf(window_title+" - %s", APP_NAME)
	m.window.SetTitle(window_title)
}

// Builds the string that will serve as the info button text.
func (m *MainInterface) getInfoButtonText() string {
	buttonText := m.session.Date.Format("1/2/2006")
	number, _ := m.boundNumber.Get()
	title, _ := m.boundTitle.Get()
	numAsInt, _ := strconv.Atoi(number)
	if numAsInt > backend.NO_SESSION_NUMBER {
		buttonText += " Session " + strconv.Itoa(m.session.SessionNumber)
		if title != "" {
			buttonText += ":"
		}
	}
	if title != "" {
		buttonText += " " + m.session.SessionTitle
	}

	return buttonText
}

// Handles the actions taken when the session info button is tapped.
func (m *MainInterface) HandleSessionInfoButton() {
	titleEntry := widget.NewEntryWithData(m.boundTitle)
	numberEntry := widget.NewEntryWithData(m.boundNumber)
	numberEntry.Validator = backend.ValidateSessionNumber

	titleForm := widget.NewFormItem("Session title", titleEntry)
	numberForm := widget.NewFormItem("Session number", numberEntry)
	formSize := fyne.NewSize(m.window.Canvas().Size().Width*0.8, m.window.Canvas().Size().Height*0.5)
	callback := func(confirm bool) {
		if confirm {
			number, _ := m.boundNumber.Get()
			numAsInt, _ := strconv.Atoi(number)
			if numAsInt != m.session.SessionNumber {
				m.session.SessionNumber = numAsInt
			}
			m.infoButton.SetText(m.getInfoButtonText())
		}
	}
	dialog := dialog.NewForm("", "Confirm", "Cancel", []*widget.FormItem{titleForm, numberForm}, callback, m.window)
	dialog.Resize(formSize)
	dialog.Show()
}

func (m *MainInterface) BindSessionInfo() {
	m.boundTitle = binding.BindString(&m.session.SessionTitle)
	m.boundNumber = binding.NewString()
	m.boundNumber.Set(strconv.Itoa(m.session.SessionNumber))
}

// Create an interface. This interface composes the entire window.
func NewMainInterface(window fyne.Window) *MainInterface {
	session := backend.NewSession(DEFAULT_SESSION_NAME, backend.NO_SESSION_NUMBER)
	mi := &MainInterface{session: session, window: window}
	textEntry := gui.NewEnterEntry(mi.session)
	mi.entry = textEntry
	mi.ExtendBaseWidget(mi)
	return mi
}

// Apply custom settings to the window.
func setUpWindow(window fyne.Window) {
	main := NewMainInterface(window)
	main.window.SetTitle(main.session.SessionTitle + " - " + APP_NAME)
	main.window.SetContent(main)
	main.window.Resize(fyne.NewSize(STARTING_WIDTH, STARTING_HEIGHT))
	main.window.Canvas().Focus(main.entry)
}

func main() {
	a := app.New()
	w := a.NewWindow(APP_NAME)
	setUpWindow(w)
	w.ShowAndRun()
}
