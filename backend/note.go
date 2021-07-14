package backend

import (
	"time"
)

// Represents an entry into the session log made by the user.
type Note struct {
	content string    // the contents of the note as input by a user
	time    time.Time // the time at which the note was created
}

// Create a new Note.
func newNote(content string, currentTime time.Time) Note {
	note := Note{
		content: content,
		time:    currentTime,
	}
	return note
}
