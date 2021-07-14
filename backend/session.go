package backend

import (
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
	s.notes = append(s.notes, n)
}
