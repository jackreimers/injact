namespace Injact.Godot;

public static class ObjectBindingExtensions
{
    public static ObjectBindingBuilder FromNode(this ObjectBindingBuilder builder, Node node)
    {
        return builder.FromInstance(node);
    }
}