namespace tor_tools
{
    class NodeOutput
    {
        public string node;
        public string item;
        public string parent;
        public string name;
        public string value;

        public NodeOutput(string node, string item, string parent, string name, string value)
        {
            this.node = node;
            this.item = item;
            this.parent = parent;
            this.name = name;
            this.value = value;
        }
    }
}
