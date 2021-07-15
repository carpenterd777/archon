package main

import (
	"testing"

	. "github.com/onsi/ginkgo"
	. "github.com/onsi/gomega"
)

func TestArchon(t *testing.T) {
	RegisterFailHandler(Fail)
	RunSpecs(t, "Archon Suite")
}
