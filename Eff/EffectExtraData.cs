namespace Pom.Eff;

public sealed class EffectExtraData
{
	public RoomSettings.RoomEffect Effect { get; private set; }
	public Dictionary<string, string> RawData { get; private set; }
	public EffectDefinition Definition { get; private set; }

	internal Dictionary<string, (IntField, Cached<int>)> _ints = new();
	internal Dictionary<string, (FloatField, Cached<float>)> _floats = new();
	internal Dictionary<string, (BoolField, Cached<bool>)> _bools = new();
	internal Dictionary<string, (StringField, Cached<string>)> _strings = new();
	public EffectExtraData(
		RoomSettings.RoomEffect effect,
		Dictionary<string, string> rawData,
		EffectDefinition definition)
	{
		Effect = effect;
		RawData = rawData;
		Definition = definition;
		foreach (var kvp in definition._fields)
		{
			string fieldname = kvp.Key;
			EffectField fielddef = kvp.Value;
			rawData.TryGetValue(fieldname, out string? fieldstringvalue);
			fieldstringvalue ??= "";
			// if (!definition.fields.TryGetValue(fieldname, out var fielddef))
			// {
			// 	fielddef = new StringField(fieldname, "");
			// }
			plog.LogDebug(fielddef);
			plog.LogDebug(fieldstringvalue);

			switch (fielddef)
			{
			case (IntField field):
			{
				if (!int.TryParse(fieldstringvalue, out var result)) result = field.DefaultInt;
				_ints[fieldname] = (field, new(result, (newval) => rawData[fieldname] = newval.ToString()));
				break;
			}
			case (FloatField field):
			{
				if (!float.TryParse(fieldstringvalue, out var result)) result = field.DefaultFloat;
				_floats[fieldname] = (field, new(result, (newval) => rawData[fieldname] = newval.ToString()));
				break;
			}
			case (BoolField field):
			{
				if (!bool.TryParse(fieldstringvalue, out var result)) result = field.DefaultBool;
				_bools[fieldname] = (field, new(result, (newval) => rawData[fieldname] = newval.ToString()));
				break;
			}
			case (StringField field):
			{
				//todo: escapes
				_strings[fieldname] = (field, new(fieldstringvalue, (newval) => rawData[fieldname] = newval ?? field.DefaultString ?? ""));
				break;
			}
			default:
			{
				plog.LogWarning($"Eff: Invalid default data setup for field {fieldname} : {fielddef} : {fielddef.DefaultValue}. Discarding");
				//_strings[fieldname] = (field, new(fieldstringvalue, (newval) => rawData[fieldname] = newval));
				break;
			}
			}
		}
	}
}

#pragma warning restore 1591