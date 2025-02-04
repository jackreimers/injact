namespace Injact.Godot.Utilities.Extensions;

public static class ObjectBindingExtensions
{
    public static ObjectBindingBuilder FromNode(this ObjectBindingBuilder builder, Node node)
    {
        return builder.FromInstance(node);
    }
}