package backend

import (
	"encoding/json"
	"errors"
	"strconv"
	"strings"
	"time"
)

// Represents a single note-taking session.
type Session struct {
	Notes         []Note    // the collection of all notes created by the user
	Date          time.Time // the date and time this session began
	SessionTitle  string    // the name of the session, if one exists
	SessionNumber int       // the number of the session, if one exists
}

// An option to customize the constructor for creating a new session.
type NewSessionOption func(s *Session)

// Create a new Session.
func NewSession(sessionTitle string, sessionNumber int, options ...NewSessionOption) *Session {
	session := Session{
		Notes:         make([]Note, 0),
		Date:          time.Now(),
		SessionTitle:  sessionTitle,
		SessionNumber: sessionNumber,
	}
	for _, options := range options {
		options(&session)
	}
	return &session
}

// Option to create a new session with a custom date field. Primarily for use in testing.
func withCustomDate(t time.Time) NewSessionOption {
	return func(s *Session) {
		s.Date = t
	}
}

// Adds a note to this session.
func (s *Session) AddNote(n Note) {
	// if the note is empty, it is likely user error
	if n.Content == "" {
		return
	}
	s.Notes = append(s.Notes, n)
}

// Returns a JSON reprsentation of the session for purposes of serialization.
func (s *Session) ToJSON() string {
	builder := new(strings.Builder)
	encoder := json.NewEncoder(builder)
	encoder.Encode(s)
	return builder.String()
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
