package gui

import (
	"time"

	"fyne.io/fyne/v2"
	"fyne.io/fyne/v2/canvas"
	"fyne.io/fyne/v2/theme"
	"fyne.io/fyne/v2/widget"
	"github.com/archon/backend"
)

const MIN_WIDTH = 400
const MIN_HEIGHT = 40

const DATE_TEXT_FACTOR = 0.7

// Handles the rendering for NoteBoxes. Implements the fyne.WidgetRenderer interface.
type NoteBoxRenderer struct {
	noteContentText *canvas.Text        // the text of the content of the note
	noteTimeText    *canvas.Text        // the text displaying the date and time the note was taken
	objects         []fyne.CanvasObject // a list of the objects declared above
	noteBox         *NoteBox            // reference to the note box being rendered
}

// The minimum size of a NoteBox. Necessary to implement the fyne.WidgetRenderer interface.
func (nbr *NoteBoxRenderer) MinSize() fyne.Size {
	return fyne.NewSize(MIN_WIDTH, MIN_HEIGHT)
}

// Position and resize the items within the NoteBox based on the input size. Necessary to implement the fyne.WidgetRenderer interface.
func (nbr *NoteBoxRenderer) Layout(size fyne.Size) {
	nbr.noteContentText.Move(fyne.NewPos(0+theme.Padding(), size.Height/3))
	nbr.noteTimeText.TextSize = theme.TextSize() * DATE_TEXT_FACTOR
	nbr.noteTimeText.Move(fyne.NewPos(size.Width-theme.Padding(), nbr.noteContentText.Position().Y))
}

// Triggers when the NoteBox changes or the theme is altered. Necessary to implement the fyne.WidgetRenderer interface.
func (nbr *NoteBoxRenderer) Refresh() {
	nbr.Layout(nbr.noteBox.Size())
	canvas.Refresh(nbr.noteBox)
	nbr.noteContentText.Text = nbr.noteBox.note.Content
	nbr.noteTimeText.Text = nbr.noteBox.note.Time.Format("Jan 2 3:04 PM")
}

// Returns the list of objects this renderer renders. Necessary to implement the fyne.WidgetRenderer interface.
func (nbr *NoteBoxRenderer) Objects() []fyne.CanvasObject {
	return nbr.objects
}

// Called when this renderer is no longer needed. Necessary to implement the fyne.WidgetRenderer interface.
func (nbr *NoteBoxRenderer) Destroy() {
	// no-op, no resources to close
}

// Apply the current theme to this element.
func (nbr *NoteBoxRenderer) ApplyTheme() {
	nbr.noteContentText.Color = theme.ForegroundColor()
	nbr.noteTimeText.Color = theme.DisabledColor()
	nbr.Refresh()
}

// A box that displays a user's Note after they have entered it. Implements the fyne.Widget interface.
type NoteBox struct {
	widget.BaseWidget
	note backend.Note
}

// Creates a NoteBox renderer. Necessary to implement the fyne.Widget interface.
func (nb *NoteBox) CreateRenderer() fyne.WidgetRenderer {
	contentText := canvas.NewText(nb.note.Content, theme.ForegroundColor())
	contentText.Alignment = fyne.TextAlignLeading

	timeText := canvas.NewText(nb.note.Time.Format("Jan 2 3:04 PM"), theme.DisabledColor())
	timeText.Alignment = fyne.TextAlignTrailing

	objects := []fyne.CanvasObject{contentText, timeText}
	return &NoteBoxRenderer{
		noteContentText: contentText,
		noteTimeText:    timeText,
		objects:         objects,
		noteBox:         nb,
	}
}

// Set the text of the content of a notebox.
func (nb *NoteBox) SetContent(content string) {
	nb.note.Content = content
	nb.Refresh()
}

// Set the time of a notebox's timestamp.
func (nb *NoteBox) SetTime(time time.Time) {
	nb.note.Time = time
	nb.Refresh()
}

// Creates a new NoteBox.
func NewNoteBox(content string, time time.Time) *NoteBox {
	note := backend.NewNote(content, time)
	nb := &NoteBox{note: note}
	nb.ExtendBaseWidget(nb)
	return nb
}
