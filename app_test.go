package main

import (
	"fyne.io/fyne/v2"
	"fyne.io/fyne/v2/test"
	"github.com/archon/gui"
	. "github.com/onsi/ginkgo"
	. "github.com/onsi/gomega"
)

var _ = Describe("App", func() {
	var app fyne.App
	var window fyne.Window

	BeforeEach(func() {
		app = test.NewApp()
		window = app.NewWindow(APP_NAME)
	})

	It("should run without crashing", func() {
		runApp := func() {
			setUpWindow(window)
		}
		Expect(runApp).ShouldNot(Panic())
	})

	It("should start up focused on the entry field", func() {
		setUpWindow(window)
		Expect(window.Canvas().Focused()).To(BeAssignableToTypeOf(&gui.EnterEntry{}))
	})

	It("should open the window at the expected starting size", func() {
		setUpWindow(window)
		Expect(window.Canvas().Size()).To(Equal(fyne.NewSize(STARTING_WIDTH, STARTING_HEIGHT)))
	})
})
