package backend

import (
	"time"

	. "github.com/onsi/ginkgo"
	. "github.com/onsi/gomega"
)

var _ = Describe("Sessions", func() {
	It("should create a Session without crashing", func() {
		createSession := func() {
			newSession("Test", 1)
		}
		Expect(createSession).ShouldNot(Panic())
	})

	It("should add a note to a session", func() {
		session := newSession("Test", 1)
		note := newNote("Test note", time.Now())

		session.addNote(note)
		Expect(session.notes).To(ContainElement(note))
	})

	It("should not add empty notes to the session", func() {
		session := newSession("Test", 1)
		note := newNote("", time.Now())

		session.addNote(note)
		Expect(session.notes).ToNot(ContainElement(note))
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
