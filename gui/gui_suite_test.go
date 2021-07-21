package gui

import (
	"testing"

	. "github.com/onsi/ginkgo"
	. "github.com/onsi/gomega"
)

func TestGUI(t *testing.T) {
	RegisterFailHandler(Fail)
	RunSpecs(t, "Gui Suite")
}
