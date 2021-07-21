package gui

import (
	"fyne.io/fyne/v2"
	"fyne.io/fyne/v2/test"
	"github.com/archon/backend"
	. "github.com/onsi/ginkgo"
	. "github.com/onsi/gomega"
)

var _ = Describe("EnterEntry widget", func() {
	var session *backend.Session
	var entry *EnterEntry

	BeforeEach(func() {
		session = backend.NewSession("Untitled Session", 0)
		entry = NewEnterEntry(session)
	})

	It("should render without crashing", func() {
		render := func() {
			test.NewWindow(entry)
		}
		Expect(render).ToNot(Panic())
	})

	It("should clear the text from the field when Enter is pressed", func() {
		test.NewWindow(entry)
		test.Type(entry, "Hello world!")
		entry.TypedKey(&fyne.KeyEvent{Name: fyne.KeyReturn})
		Expect(entry.Text).To(BeEmpty())
	})

	It("should not clear the text from the field if Enter is not pressed", func() {
		test.NewWindow(entry)
		note := "Hello world!"
		test.Type(entry, note)
		Expect(entry.Text).To(Equal(note))
	})

	It("should modify the session such that a new note exists when the user presses Enter", func() {
		test.NewWindow(entry)
		note := "Hello world!"
		test.Type(entry, note)
		entry.TypedKey(&fyne.KeyEvent{Name: fyne.KeyReturn})
		Expect(session.Notes).To(HaveLen(1))
		Expect(session.Notes[0].Content).To(Equal(note))
	})

	It("should not modify the session such that a new note exists when the user does not press Enter", func() {
		test.NewWindow(entry)
		note := "Hello world!"
		test.Type(entry, note)
		Expect(session.Notes).To(BeEmpty())
	})
})
