using System.Xml.Linq;

namespace Xlf.Core.Test;

public class UnitFiltererTest
{
    private static XElement GetXliffDocumentFile()
    {
        XElement e = new(XlfHelper.XLIFF_NS + "file");
        _ = new XDocument(
            new XElement(XlfHelper.XLIFF_NS + "xliff", e));
        return e;
    }

    private static XElement GetXliffUnit(string source, string? tag = null)
    {
        XElement unit = new(XlfHelper.XLIFF_NS + "unit",
            new XElement(XlfHelper.XLIFF_NS + "segment",
                new XElement(XlfHelper.XLIFF_NS + "source", source)));

        if (tag != null)
        {
            unit.Add(new XElement(XlfHelper.XLIFF_NS + "notes",
                new XElement(XlfHelper.XLIFF_NS + "note",
                    new XAttribute("category", "description"), tag)));
        }

        return unit;
    }

    [Fact]
    public void Filter_UntaggedUnit_Removed()
    {
        // arrange
        XElement file = GetXliffDocumentFile();
        file.Add(GetXliffUnit("hello"));

        // act
        int removed = UnitFilterer.Filter(file.Document!, "tag");

        // assert
        Assert.Equal(1, removed);
        Assert.Empty(file.Descendants(XlfHelper.XLIFF_NS + "unit"));
    }

    [Fact]
    public void Filter_UntaggedUnitWithPreserve_NotRemoved()
    {
        // arrange
        XElement file = GetXliffDocumentFile();
        file.Add(GetXliffUnit("hello"));

        // act
        int removed = UnitFilterer.Filter(file.Document!, "tag", true);

        // assert
        Assert.Equal(0, removed);
        Assert.Single(file.Descendants(XlfHelper.XLIFF_NS + "unit"));
    }

    [Fact]
    public void Filter_Tagged_Preserved()
    {
        // arrange
        XElement file = GetXliffDocumentFile();

        // untagged
        file.Add(GetXliffUnit("hello"));

        // tagged with preserved tag
        file.Add(GetXliffUnit("world", "white"));

        // tagged with another value
        file.Add(GetXliffUnit("world", "black"));

        // act
        int removed = UnitFilterer.Filter(file.Document!, "white");

        // assert
        Assert.Equal(2, removed);
        Assert.Single(file.Descendants(XlfHelper.XLIFF_NS + "unit"));
    }
}