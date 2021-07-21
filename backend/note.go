package backend

import (
	"time"
)

// Represents an entry into the session log made by the user.
type Note struct {
	Content string    // the contents of the note as input by a user
	Time    time.Time // the time at which the note was created
}

// Create a new Note.
func NewNote(content string, currentTime time.Time) Note {
	note := Note{
		Content: content,
		Time:    currentTime,
	}
	return note
}
