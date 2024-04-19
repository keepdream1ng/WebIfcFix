using GeometryGym.Ifc;

namespace IfcFixLib;

public class IfcFilter
{
    public string SearchString { get; set; }
    public StringChecker Checker { get; set; }

    public int FilterIfcString(DatabaseIfc db, out string ifcString)
    {
        ifcString = string.Empty;
        DuplicateOptions options = new DuplicateOptions(db.Tolerance);
        options.DuplicateDownstream = false;
        DatabaseIfc newDb = new DatabaseIfc(db);
        IfcProject project = newDb.Factory.Duplicate(db.Project, options) as IfcProject;

        options.DuplicateDownstream = true;

        List<IfcBuiltElement> modelElements = db.Project.Extract<IfcBuiltElement>();
        int count = 0;
        foreach(IfcBuiltElement el in modelElements)
        {
            if (Checker.Check(el.Name, SearchString))
            {
                count++;
                newDb.Factory.Duplicate(el, options);
            }
        }
        modelElements.Clear();

        if (count > 0)
        {
            ifcString = newDb.ToString(FormatIfcSerialization.STEP)!;
        }
        db.Dispose();
        newDb.Dispose();
        return count;
    }

}

