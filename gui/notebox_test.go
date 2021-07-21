package gui

import (
	"time"

	"fyne.io/fyne/v2/test"
	. "github.com/onsi/ginkgo"
	. "github.com/onsi/gomega"
)

var _ = Describe("Notebox", func() {
	It("should render without crashing", func() {
		notebox := NewNoteBox("Hello world!", time.Now())
		render := func() {
			test.NewWindow(notebox)
		}
		Expect(render).ToNot(Panic())
	})
})
