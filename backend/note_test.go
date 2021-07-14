package backend

import (
	"time"

	. "github.com/onsi/ginkgo"
	. "github.com/onsi/gomega"
)

var _ = Describe("App", func() {
	It("should create a Note without crashing", func() {
		runApp := func() {
			newNote("Test note", time.Now())
		}
		Expect(runApp).ShouldNot(Panic())
	})
})
