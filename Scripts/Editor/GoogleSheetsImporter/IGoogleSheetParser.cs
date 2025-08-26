namespace Game.Scripts.Editor.GoogleSheetsImporter
{
    public interface IGoogleSheetParser
    {
        public void Parse(string header, string token);
    }
}