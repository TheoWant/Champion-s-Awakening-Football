using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot]
public class SaveObject
{
    [XmlElement]
    public LeagueCalendar calendarData;

    [XmlElement]
    public List<CardMatch> cardMatchList;
}
