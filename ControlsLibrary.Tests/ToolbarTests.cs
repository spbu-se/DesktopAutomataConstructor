using ControlsLibrary.Controls.Toolbar;
using ControlsLibrary.Model;
using NUnit.Framework;

namespace ControlsLibrary.Tests
{
    public class Tests
    {
        private ToolbarViewModel toolbar;

        [SetUp]
        public void Setup()
        {
            toolbar = new ToolbarViewModel();
        }

        [Test]
        public void DeletionToolSelectedTest()
        {
            toolbar.DeleteToolSelected = true;
            Assert.False(toolbar.EditToolSelected || toolbar.SelectToolSelected);
            Assert.AreEqual(SelectedTool.Delete, toolbar.SelectedTool);
            Assert.True(toolbar.DeleteToolSelected);
        }

        [Test]
        public void SelectionToolSelectedTest()
        {
            toolbar.SelectToolSelected = true;
            Assert.False(toolbar.EditToolSelected || toolbar.DeleteToolSelected);
            Assert.AreEqual(SelectedTool.Select, toolbar.SelectedTool);
            Assert.True(toolbar.SelectToolSelected);
        }

        [Test]
        public void EditingToolSelectedTest()
        {
            toolbar.DeleteToolSelected = true;
            Assert.False(toolbar.EditToolSelected || toolbar.SelectToolSelected);
            Assert.AreEqual(SelectedTool.Delete, toolbar.SelectedTool);
            Assert.True(toolbar.DeleteToolSelected);
        }

        [Test]
        public void SelectedToolChangedTest()
        {
            var eventExecuted = false;
            toolbar.SelectedToolChanged += (sender, e) => eventExecuted = true;
            toolbar.DeleteToolSelected = true;

            Assert.True(eventExecuted);
            eventExecuted = false;

            toolbar.SelectToolSelected = true;
            Assert.False(toolbar.EditToolSelected || toolbar.DeleteToolSelected);
            Assert.AreEqual(SelectedTool.Select, toolbar.SelectedTool);
            Assert.True(toolbar.SelectToolSelected);

            Assert.True(eventExecuted);
        }
    }
}