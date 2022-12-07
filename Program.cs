using System.Text;

class ListNode
{
    public ListNode Prev;
    public ListNode Next;
    public ListNode Rand; // произвольный элемент внутри списка
    public string Data;
}

class ListRand
{
    public ListNode Head;
    public ListNode Tail;
    public int Count;

    public void Serialize(FileStream s)
    {
        StringBuilder stringBuilder = new StringBuilder();
        const string SEPARATOR = "/|";
        const string EMPTY_REF = "-1";
        Dictionary<ListNode, int> nodeDictionary;
        ListNode currentNode;
        
        PrepareData();
        SerializeData();
        Console.WriteLine("Saved!");

        void PrepareData()
        {
            nodeDictionary = new(Count);
            int currentIndex = 0;
            for (currentNode = Tail; currentNode != null; currentNode = currentNode.Next)
            {
                nodeDictionary[currentNode] = currentIndex;
                currentIndex++;
            }
        }

        void SerializeData()
        {
            using (StreamWriter writer = new StreamWriter(s))
            {
                for (currentNode = Tail; currentNode != null; currentNode = currentNode.Next)
                {
                    stringBuilder.Clear();
                    AppendNodeIndex(currentNode.Prev);
                    AppendNodeIndex(currentNode.Next);
                    AppendNodeIndex(currentNode.Rand);
                    AppendNodeData(currentNode);
                    writer.WriteLine(stringBuilder);
                }
            }
        }
        
        void AppendNodeIndex(ListNode node)
        {
            if(node != null)
                stringBuilder.Append(nodeDictionary[node] + SEPARATOR);
            else
                stringBuilder.Append(EMPTY_REF + SEPARATOR);
        }
        
        void AppendNodeData(ListNode node)
        {
            if(string.IsNullOrEmpty(node.Data) == false)
                stringBuilder.Append(node.Data + SEPARATOR);
            else
                stringBuilder.Append(EMPTY_REF + SEPARATOR);
        }
    }

    public void Deserialize(FileStream s)
    {
        const string SEPARATOR = "/|";
        const string EMPTY_REF = "-1";
        List<ListNode> nodes = new List<ListNode>();
        Count = 0;

        try
        {
            using (StreamReader reader = new StreamReader(s))
            {
                CreateNodeInstances(reader);
                s.Seek(0, SeekOrigin.Begin);
                FillAllNodes(reader);

                Tail = nodes[0];
                Head = nodes[^1];
                Console.WriteLine($"Loaded!");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Can't deserialize linked list.");
            Console.WriteLine(e.Message);
        }

        void CreateNodeInstances(StreamReader reader)
        {
            while (!string.IsNullOrEmpty(reader.ReadLine()))
            {
                nodes.Add(new ListNode());
                Count++;
            }
        }
        
        void FillAllNodes(StreamReader reader)
        {
            int currentIndex = 0;
            string line;
            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                string[] data = line.Split(SEPARATOR);
                FillNodeData(nodes[currentIndex], data);
                currentIndex++;
            }
        }
        
        void FillNodeData(ListNode node, string[] data)
        {
            node.Prev = GetNodeFromIndex(data[0]);
            node.Next = GetNodeFromIndex(data[1]);
            node.Rand = GetNodeFromIndex(data[2]);

            if (data[3].Equals(EMPTY_REF) == false)
                node.Data = data[3];
        }
        
        ListNode GetNodeFromIndex(string data)
        {
            if (int.TryParse(data, out int linkIndex))
                if (data.Equals(EMPTY_REF) == false)
                    return nodes[linkIndex];
            return null;
        }
    }
}

#region Testing code
class Program
{
    private const string _savePath = @"C:\Users\mirror\Desktop\data.txt";
    
    public static void Main(string[] args)
    {
        ListRand list = new ListRand();
        ListNode n1 = new ListNode();
        ListNode n2 = new ListNode();
        ListNode n3 = new ListNode();
        ListNode n4 = new ListNode();
        
        n1.Next = n2;
        n2.Next = n3;
        n3.Next = n4;
        n4.Next = null;
        
        n1.Prev = null;
        n2.Prev = n1;
        n3.Prev = n2;
        n4.Prev = n3;
        
        n1.Rand = n3;
        n2.Rand = n2;
        n3.Rand = n4;
        n4.Rand = n2;

        n1.Data = "My custom data, N1";
        n2.Data = "k8374919839k";
        n3.Data = "dwarfs";
        n4.Data = @"Myasdkj.,zvcx123|'/LAJHSDJKHjNFdiuFADIOFDJLK\]afsdasdf";

        list.Count = 4;
        list.Tail = n1;
        list.Head = n4;
        
        using (FileStream s = new FileStream(_savePath, FileMode.Create))
        {
            list.Serialize(s);
        }
        
        using (FileStream s = new FileStream(_savePath, FileMode.Open))
        {
            list.Deserialize(s);
        }

        Console.WriteLine("Press any key...");
        Console.ReadKey();
    }
}
#endregion