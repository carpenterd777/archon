package backend

import (
	"strconv"
	"time"

	. "github.com/onsi/ginkgo"
	. "github.com/onsi/gomega"
)

var _ = Describe("Sessions", func() {
	It("should create a Session without crashing", func() {
		createSession := func() {
			NewSession("Test", 1)
		}
		Expect(createSession).ShouldNot(Panic())
	})

	It("should add a note to a session", func() {
		session := NewSession("Test", 1)
		note := NewNote("Test note", time.Now())

		session.AddNote(note)
		Expect(session.Notes).To(ContainElement(note))
	})

	It("should not add empty notes to the session", func() {
		session := NewSession("Test", 1)
		note := NewNote("", time.Now())

		session.AddNote(note)
		Expect(session.Notes).ToNot(ContainElement(note))
	})
})

var _ = Describe("Session number validator", func() {
	It("should allow positive session numbers", func() {
		Expect(ValidateSessionNumber("1")).To(BeNil())
	})

	It("should allow empty input", func() {
		Expect(ValidateSessionNumber("")).To(BeNil())
	})

	It("should not allow negative numbers", func() {
		Expect(ValidateSessionNumber("-1")).ToNot(BeNil())
	})

	It("should not allow non-numeric characters", func() {
		Expect(ValidateSessionNumber("nan")).ToNot(BeNil())
	})

	It("should not allow float numbers", func() {
		Expect(ValidateSessionNumber("1.23")).ToNot(BeNil())
	})
})

var _ = Describe("JSON Serialization", func() {
	It("should serialize the session title", func() {
		title := "The Conquest at Calimport"
		s := NewSession(title, 0)
		Expect(s.ToJSON()).To(ContainSubstring("\"SessionTitle\":\"" + title + "\""))
	})

	It("should serialze the session number", func() {
		number := 9
		s := NewSession("Reunion in the Face of Adversity", number)
		Expect(s.ToJSON()).To(ContainSubstring("\"SessionNumber\":" + strconv.Itoa(number)))
	})

	It("should serialize the session date", func() {
		date := time.Date(2021, time.June, 22, 15, 0, 0, 0, time.FixedZone("UTC-0", 0))
		s := NewSession("The Conquest at Calimport", 0, withCustomDate(date))
		Expect(s.ToJSON()).To(ContainSubstring("\"Date\":\"2021-06-22T15:00:00Z\""))
	})

	It("should serialize with no notes if none were added", func() {
		s := NewSession("The Return of Aust Redwyn", 0)
		Expect(s.ToJSON()).To(ContainSubstring("\"Notes\":[]"))
	})

	It("should serialize with notes if any were added", func() {
		s := NewSession("The Conquest at Calimport", 0)
		date := time.Date(2021, time.June, 22, 15, 0, 0, 0, time.FixedZone("UTC-0", 0))
		s.AddNote(NewNote("Xenthe almost died", date))
		Expect(s.ToJSON()).To(ContainSubstring("[{\"Content\":\"Xenthe almost died\",\"Time\":\"2021-06-22T15:00:00Z\"}]"))
	})
})
