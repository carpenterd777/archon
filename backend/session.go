package backend

import (
	"errors"
	"strconv"
	"time"
)

// Represents a single note-taking session.
type Session struct {
	notes         []Note    // the collection of all notes created by the user
	date          time.Time // the date and time this session began
	sessionTitle  string    // the name of the session, if one exists
	sessionNumber int       // the number of the session, if one exists
}

// Create a new Session.
func newSession(sessionTitle string, sessionNumber int) Session {
	session := Session{
		notes:         make([]Note, 0),
		date:          time.Now(),
		sessionTitle:  sessionTitle,
		sessionNumber: sessionNumber,
	}
	return session
}

// Adds a note to this session.
func (s *Session) addNote(n Note) {
	// if the note is empty, it is likely user error
	if n.content == "" {
		return
	}
	s.notes = append(s.notes, n)
}

// Ensures that a session number entered by a user is non-negative.
//
// The function signature is modeled to be a fyne.StringValidator.
func ValidateSessionNumber(sessionNumber string) error {
	if sessionNumber == "" {
		return nil
	}

	i, err := strconv.Atoi(sessionNumber)

	if err != nil {
		return err
	}

	if i < 0 {
		return errors.New("Session numbers cannot be less than 0")
	}

	return nil
}
