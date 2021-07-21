package gui

import (
	"time"

	"fyne.io/fyne/v2"
	"fyne.io/fyne/v2/widget"
	"github.com/archon/backend"
)

// An Entry field that submits some text when the Enter key is pressed while this is focused. Multiline by default.
type EnterEntry struct {
	widget.Entry
	session *backend.Session // a session state that this entry is allowed to modify
}

// Handler for enter key presses. Clears the text in the entry.
func (e *EnterEntry) onEnter() {
	e.session.AddNote(backend.NewNote(e.Text, time.Now()))
	e.Entry.SetText("")
}

// Overrides the TypedKey method of the fyne.Focusable interface.
func (e *EnterEntry) TypedKey(key *fyne.KeyEvent) {
	switch key.Name {
	case fyne.KeyEnter:
		e.onEnter()
	case fyne.KeyReturn:
		e.onEnter()
	default:
		e.Entry.TypedKey(key)
	}
}

// Sets this entry's seesion to the passed session.
func (e *EnterEntry) SetSession(session *backend.Session) {
	e.session = session
}

// Creates a new Entry that adds a note to the passed session when Enter is pressed.
func NewEnterEntry(session *backend.Session) *EnterEntry {
	entry := &EnterEntry{session: session}
	entry.Entry.MultiLine = true
	entry.Entry.Wrapping = fyne.TextWrapWord
	entry.ExtendBaseWidget(entry)
	return entry
}
