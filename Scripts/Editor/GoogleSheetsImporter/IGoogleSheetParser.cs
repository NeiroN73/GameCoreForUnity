namespace GameCore.GoogleSheetsImporter
{
    public abstract class GoogleSheetParser<T> : GoogleSheetParser
    {
        public T Data { get; }
    }

    public abstract class GoogleSheetParser
    {
        public abstract string SheetName { get; }
        public abstract void Parse(string header, string token);
    }
}