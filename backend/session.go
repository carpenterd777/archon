package backend

import (
	"encoding/json"
	"errors"
	"io/fs"
	"os"
	"strconv"
	"strings"
	"time"
)

const NO_SESSION_NUMBER = -1

// Represents a single note-taking session.
type Session struct {
	Notes         []Note    // the collection of all notes created by the user
	Date          time.Time // the date and time this session began
	SessionTitle  string    // the name of the session, if one exists
	SessionNumber int       // the number of the session, if one exists
	Path          string    // the path to the file where this session is saved, if one exists
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
		Path:          "",
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

// Builds a session from a JSON representation of a session.
// If the input string is invalid, returns a pointer to an empty Session and an error.
func FromJSON(s string) (*Session, error) {
	reader := strings.NewReader(s)
	decoder := json.NewDecoder(reader)
	session := Session{}
	err := decoder.Decode(&session)

	if err != nil {
		return &Session{}, err
	}

	// rectify invalid data modified externally outside of the application
	if session.SessionNumber < NO_SESSION_NUMBER {
		session.SessionNumber = NO_SESSION_NUMBER
	}

	return &session, nil
}

// Writes Session data to specified file.
func (s *Session) Save() error {
	UserRW := fs.FileMode(0600)
	return os.WriteFile(s.Path, []byte(s.ToJSON()), UserRW)
}

// Load Session data from specified file.
// In the case of an error during reading the file or converting it into a session,
// returns an empty session and an error.
func Load(path string) (*Session, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return &Session{}, err
	}

	s, err := FromJSON(string(data))
	if err != nil {
		return &Session{}, err
	}

	return s, nil

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

	if i < NO_SESSION_NUMBER {
		return errors.New("Session numbers cannot be less than 0")
	}

	return nil
}
