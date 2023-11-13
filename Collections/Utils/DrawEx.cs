namespace Collections;

// Some weird attempts

public class Draw3
{
    public static readonly Dictionary<string, Draw3> cache = new();
    private List<(ImGuiCol col, Vector4 color)> styleStack { get; set; } = new();
    private (ImGuiCol col, Vector4 color)? color { get; set; }
    private Vector2? offset { get; set; }
    private Vector2? originalPos { get; set; }

    //public Draw3(string key)
    //{
    //    cache[key] = this;
    //}

    public static Draw3 Get(string key, (ImGuiCol col, Vector4 color)? color = null, Vector2? offset = null)
    {
        if (cache.ContainsKey(key))
        {
            return cache[key];
        }
        return cache[key] = new Draw3() { offset = offset, color = color };
        //return cache[key] = new Draw3(key);
    }

    //public Draw3 WithColor(ImGuiCol col, Vector4 color)
    //{
    //    styleStack.Add((col, color));
    //    return this;
    //}

    //public Draw3 WithOffset(Vector2 offset)
    //{
    //    this.offset = offset;
    //    return this;
    //}

    public Draw3 Start()
    {
        if (offset is not null)
        {
            originalPos = ImGui.GetCursorPos();
            ImGui.SameLine();
            ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPos().X - offset.Value.X, ImGui.GetCursorPos().Y - offset.Value.Y));
        }

        if (color is not null)
        {
            ImGui.PushStyleColor(color.Value.col, color.Value.color);
        }

        return this;
    }

    public void Reset()
    {
        if (originalPos is not null)
            ImGui.SetCursorPos((Vector2)originalPos);

        if (color is not null)
            ImGui.PopStyleColor();
        //foreach (var (_, _) in styleStack)
        //{
        //    ImGui.PopStyleColor();
        //}
    }
}



public class Draw2
{

    public static readonly Dictionary<string, Draw2> cache = new();

    private List<System.Action> resetStack { get; set; } = new();
    private Vector2? originalPos { get; set; } = null;
    private int styleColorCount { get; set; } = 0;

    public Draw2(string key)
    {
        cache[key] = this;
    }
    public static Draw2 Get(string key)
    {
        if (cache.ContainsKey(key))
            return cache[key];

        return cache[key] = new Draw2(key);
    }

    public Draw2 WithColor(ImGuiCol col, Vector4 color)
    {
        styleColorCount++;
        ImGui.PushStyleColor(col, color);
        return this;
    }

    public Draw2 WithOffset(Vector2 offset)
    {
        originalPos = ImGui.GetCursorPos();
        ImGui.SameLine();
        ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPos().X - offset.X, ImGui.GetCursorPos().Y - offset.Y));
        return this;
    }

    public void Reset()
    {
        if (originalPos is not null)
            ImGui.SetCursorPos((Vector2)originalPos);

        if (styleColorCount != 0)
            ImGui.PopStyleColor(styleColorCount);

        //if (resetStack.Any())
        //    resetStack.Clear();
        //ResetState();
    }

    private void ResetState()
    {
        resetStack.Clear();
        originalPos = null;
        styleColorCount = 0;
    }

}
public class Draw
{
    private List<System.Action> styleStack { get; set; } = new();
    private Vector2? offset { get; set; } = null;
    private int? id { get; set; } = null;
    private bool sameLine { get; set; } = false;

    public static Draw Create()
    {
        return new Draw();
    }

    public Draw WithID(int id)
    {
        this.id = id;
        return this;
    }

    public Draw WithOffset(Vector2 offset)
    {
        this.offset = offset;
        return this;
    }

    public Draw WithColor(ImGuiCol col, Vector4 color)
    {
        styleStack.Add(() => ImGui.PushStyleColor(col, color));
        return this;
    }

    public Draw WithSameLine()
    {
        sameLine = true;
        return this;
    }

    public void Start(int drawItemCount, ref bool isFavorite)
    {
        UiHelper.IconButtonWithOffset(drawItemCount, FontAwesomeIcon.Star, 40, 0, ref isFavorite);
    }

    public void Start2(System.Action draw)
    {

        //if (id is not null)
        //{
        //    ImGui.PushID((int)id);
        //}

        //Vector2? previousPos = null;
        //if (offset is not null)
        //{
        var previousPos = ImGui.GetCursorPos();

        //if (sameLine)
        //{
            ImGui.SameLine();
        //}

        var offsetTyped = (Vector2)offset;
        ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPos().X - offsetTyped.X, ImGui.GetCursorPos().Y - offsetTyped.Y));
        //}

        //foreach (var action in styleStack)
        //{
        //    action();
        //}

        ImGui.PushStyleColor(ImGuiCol.Text, ColorsPalette.GREY);

        try
        {
            draw();
        }
        catch (Exception)
        {
            ResetState(previousPos);
            throw;
        }

        ResetState(previousPos);
    }

    private void ResetState(Vector2? previousPos)
    {
        //if (offset is not null)
        //{
            ImGui.SetCursorPos((Vector2)previousPos);
        //}

        ImGui.PopStyleColor(styleStack.Count);

        //if (id is not null)
        //{
        //    ImGui.PopID();
        //}
    }
}
