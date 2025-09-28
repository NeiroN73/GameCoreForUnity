namespace GameCore.GoogleSheetsImporter
{
    public abstract class GoogleSheetParser
    {
        public abstract string SheetName { get; }
        public abstract void Parse(string header, string value);
    }
}