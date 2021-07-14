package archon

import (
	"fyne.io/fyne/v2/test"
	. "github.com/onsi/ginkgo"
	. "github.com/onsi/gomega"
)

var _ = Describe("App", func() {
	It("should run without crashing", func() {
		runApp := func() {
			test.NewApp()
		}
		Expect(runApp).ShouldNot(Panic())
	})
})
