using System;
using System.Linq;
using System.Xml.Linq;

namespace Xlf.Core;

public static class UnitFilterer
{
    /// <summary>
    /// Filters the specified document by removing all the unit descendants
    /// except the ones having notes/note@category=description whose value
    /// is <paramref name="tag"/>.
    /// </summary>
    /// <param name="doc">The document to filter.</param>
    /// <param name="tag">The unit tag to preserve.</param>
    /// <param name="preserveUntagged">if set to <c>true</c> preserve untagged
    /// units.</param>
    /// <returns>Count of removed units.</returns>
    /// <exception cref="ArgumentNullException">doc or tag</exception>
    public static int Filter(XDocument doc, string tag,
        bool preserveUntagged = false)
    {
        ArgumentNullException.ThrowIfNull(doc);
        ArgumentNullException.ThrowIfNull(tag);

        // remove all the unit descendants except the ones having
        // notes/note@category=description and value equal to the tag
        int removed = 0;
        foreach (XElement unitElem in doc.Descendants(XlfHelper.XLIFF_NS + "unit")
            .ToList())
        {
            // if no notes, remove the unit unless preserving untagged
            if (unitElem.Element(XlfHelper.XLIFF_NS + "notes") == null)
            {
                if (!preserveUntagged)
                {
                    unitElem.Remove();
                    removed++;
                }
            }
            // else remove the unit except when it has the tag as a note
            else
            {
                XElement? noteElem = unitElem.Descendants(
                    XlfHelper.XLIFF_NS + "note")
                    .FirstOrDefault(n => n.Attribute("category")?.Value
                        == "description" && n.Value == tag);
                if (noteElem == null)
                {
                    unitElem.Remove();
                    removed++;
                }
            }
        }

        return removed;
    }
}
