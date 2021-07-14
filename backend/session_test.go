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
})
