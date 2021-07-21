package gui

import (
	"image/color"

	"fyne.io/fyne/v2"
	"fyne.io/fyne/v2/canvas"
	"fyne.io/fyne/v2/theme"
	"fyne.io/fyne/v2/widget"
)

// Handles the rendering for SavingIndicators. Implements fyne.WidgetRenderer.
type SavingIndicatorRenderer struct {
	backgroundRect  *canvas.Rectangle   // the rectangle that exists in the background
	objects         []fyne.CanvasObject // a list of the objects listed above
	savingIndicator *SavingIndicator    // a reference to the saving indicator being rendered
}

// The minumum size of a SavingIndicator. Necessary to implement fyne.WidgetRenderer.
func (s *SavingIndicatorRenderer) MinSize() fyne.Size {
	var min_width, min_height float32 = 10, 3
	return fyne.NewSize(min_width, min_height)
}

// Position and resize the items within the SavingIndicator based on the input size. Necessary to implement the fyne.WidgetRenderer interface.
func (s *SavingIndicatorRenderer) Layout(size fyne.Size) {
	s.backgroundRect.Resize(size)
}

// Triggers when the SavingIndicator changes or the theme is altered. Necessary to implement the fyne.WidgetRenderer interface.
func (s SavingIndicatorRenderer) Refresh() {
	s.Layout(s.backgroundRect.Size())
	canvas.Refresh(s.backgroundRect)
}

// Returns the list of objects this renderer renders. Necessary to implement the fyne.WidgetRenderer interface.
func (s *SavingIndicatorRenderer) Objects() []fyne.CanvasObject {
	return s.objects
}

// Called when this renderer is no longer needed. Necessary to implement the fyne.WidgetRenderer interface.
func (s *SavingIndicatorRenderer) Destroy() {
	// no-op, no resources to close
}

// A saving indicator that plays a small animation. Implements the fyne.Widget interface.
type SavingIndicator struct {
	widget.BaseWidget
	rect *canvas.Rectangle
}

// Creates a SavingIndicator renderer. Necessary to implement the fyne.Widget interface.
func (s *SavingIndicator) CreateRenderer() fyne.WidgetRenderer {
	s.rect = canvas.NewRectangle(theme.DisabledColor())
	objects := []fyne.CanvasObject{s.rect}
	return &SavingIndicatorRenderer{
		backgroundRect:  s.rect,
		objects:         objects,
		savingIndicator: s,
	}
}

// Set the color of the indicator. For use by the animation function.
func (s *SavingIndicator) SetColor(c color.Color) {
	s.rect.FillColor = c
	s.rect.StrokeColor = c
}

// Creates a new saving indicator.
func NewSavingIndicator() *SavingIndicator {
	s := &SavingIndicator{}
	s.ExtendBaseWidget(s)
	return s
}
