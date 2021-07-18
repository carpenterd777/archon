package main

import (
	"time"

	"fyne.io/fyne/v2"
	"fyne.io/fyne/v2/app"
	"fyne.io/fyne/v2/canvas"
	"fyne.io/fyne/v2/container"
	"fyne.io/fyne/v2/widget"
	"github.com/archon/backend"
	"github.com/archon/gui"
)

const APP_NAME = "Archon"
const DEFAULT_SESSION_NAME = "Untitled Session"
const DEFAULT_SESSION_NUMBER = 0
const STARTING_WIDTH = 600
const STARTING_HEIGHT = 400

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
	session *backend.Session // The state of this application session
	entry   *gui.EnterEntry  // The entry field
}

// Creates a renderer for the main window. Necessary to implement the widget.Widget inteface.
func (m *MainInterface) CreateRenderer() fyne.WidgetRenderer {

	list := widget.NewList(
		m.listLength,
		m.listCreateItem,
		m.listUpdateItem,
	)

	cont := container.NewBorder(nil, m.entry, nil, nil, list)
	return &MainInterfaceRenderer{
		cont: cont,
		mi:   m,
	}

}

func (m *MainInterface) listLength() int {
	return len(m.session.Notes)
}

func (m *MainInterface) listCreateItem() fyne.CanvasObject {
	return gui.NewNoteBox("", time.Time{})
}

func (m *MainInterface) listUpdateItem(i widget.ListItemID, o fyne.CanvasObject) {
	o.(*gui.NoteBox).SetContent(m.session.Notes[i].Content)
	o.(*gui.NoteBox).SetTime(m.session.Notes[i].Time)
}

func NewMainInterface() *MainInterface {
	session := backend.NewSession(DEFAULT_SESSION_NAME, DEFAULT_SESSION_NUMBER)
	textEntry := gui.NewEnterEntry(session)
	mi := &MainInterface{session: session, entry: textEntry}
	mi.ExtendBaseWidget(mi)
	return mi
}

func setUpWindow(window fyne.Window) {
	main := NewMainInterface()
	window.SetTitle(main.session.SessionTitle + " - " + APP_NAME)
	window.SetContent(main)
	window.Resize(fyne.NewSize(STARTING_WIDTH, STARTING_HEIGHT))
	window.Canvas().Focus(main.entry)
}

func main() {
	a := app.New()
	w := a.NewWindow(APP_NAME)
	setUpWindow(w)
	w.ShowAndRun()
}
