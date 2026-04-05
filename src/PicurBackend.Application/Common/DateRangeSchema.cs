public static class DateRangeSchema
{
    public static string Json => """
    {
      "type": "object",
      "properties": {
        "startDate": { "type": "string", "format": "date-time" },
        "endDate": { "type": "string", "format": "date-time" }
      },
      "required": ["startDate", "endDate"]
    }
    """;
}