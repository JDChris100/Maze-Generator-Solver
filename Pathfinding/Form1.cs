using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pathfinding
{
    public enum Direction
    {
        LEFT, RIGHT, UP, DOWN
    }

    public class Node
    {
        public static List<Node> nodes = new List<Node>();

        public int x, y;
        public List<Direction> directions = new List<Direction>();
        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
            nodes.Add(this);
        }

        public static Node getnode(int x, int y)
        {
            foreach (Node n in nodes)
            {
                if (n.x == x && n.y == y)
                {
                    return n;
                }
            }
            return null;
        }
    }
    public partial class Form1 : Form
    {
        int width;
        int height;
        int cellWidth = 20;

        Random r = new Random();

        // Variables used in maze generator algorithm
        public List<string> stack;
        public List<string> grid;

        Graphics g;
        Panel panel;
        Pen pen = new Pen(new SolidBrush(Color.Black), 2);
        Pen eraser = new Pen(new SolidBrush(Color.White), 2);

        List<List<Node>> solutions = new List<List<Node>>();
        List<Node> visited = new List<Node>();

        public Form1()
        {
            InitializeComponent();
        }

        public List<Node> copy(List<Node> og)
        {
            List<Node> nodes = new List<Node>();
            foreach (Node n in og)
            {
                nodes.Add(n);
            }
            return nodes;
        }

        public void creategrid(int width, int height)
        {
            solutions.Clear();
            Node.nodes.Clear();
            stack = new List<string>();
            grid = new List<string>();
            if (panel != null)
            {
                if (Controls.Contains(panel))
                {
                    Controls.Remove(panel);
                }
                panel = null;
            }
            this.width = width;
            this.height = height;
            // Resize the form
            Width = cellWidth * (width + 1) + 40;
            Height = cellWidth * (height + 1) + 100;
            if (Width < 20 * (30 + 1) + 40)
            {
                Width = 20 * (30 + 1) + 40;
            }
            if (Height < 20 * (30 + 1) + 40)
            {
                Height = 20 * (30 + 1) + 40;
            }
            //Create Grid
            panel = new Panel();
            panel.Width = cellWidth * (width + 1);
            panel.Height = cellWidth * (height + 1);
            panel.Location = new Point(5, 5);
            panel.Paint += new PaintEventHandler(panel_paint);
            Controls.Add(panel);
            g = panel.CreateGraphics();
        }

        public void panel_paint(object sender, PaintEventArgs e)
        {
            // Create the grid

            // Draw lines Horizontally
            for (int y = 0; y <= height; y++)
            {
                g.DrawLine(pen, 0, y * cellWidth, cellWidth * width, (y) * cellWidth);
            }

            // Draw lines Vertically
            for (int x = 0; x <= width; x++)
            {
                g.DrawLine(pen, x * cellWidth, 0, (x) * cellWidth, cellWidth * height);
            }
            generate(0, 0);
        }

        public void displaynode(Node n)
        {
            g.FillRectangle(new SolidBrush(Color.Salmon), new Rectangle((n.x * cellWidth) + 3, (n.y * cellWidth) + 3, cellWidth - 6, cellWidth - 6));
        }

        public void hidenode(Node n)
        {
            g.FillRectangle(new SolidBrush(Color.White), new Rectangle((n.x * cellWidth) + 5, (n.y * cellWidth) + 5, cellWidth - 10, cellWidth - 10));
        }

        public void pathfinding(List<Node> history, Node Target, Node add)
        {
            history.Add(add);
            Node current = history[history.Count - 1];
            visited.Add(current);
            foreach (Direction dir in current.directions)
            {
                if (dir == Direction.DOWN)
                {
                    Node n = Node.getnode(current.x, current.y + 1);
                    if (n == Target)
                    {
                        history.Add(n);
                        solutions.Add(history);
                    }
                    else
                    {
                        pathfinding(copy(history), Target, n);
                    }
                }
                if (dir == Direction.UP)
                {
                    Node n = Node.getnode(current.x, current.y - 1);
                    if (n == Target)
                    {
                        history.Add(n);
                        solutions.Add(history);
                    }
                    else
                    {
                        pathfinding(copy(history), Target, n);
                    }
                }
                if (dir == Direction.LEFT)
                {
                    Node n = Node.getnode(current.x - 1, current.y);
                    if (n == Target)
                    {
                        history.Add(n);
                        solutions.Add(history);
                    }
                    else
                    {
                        pathfinding(copy(history), Target, n);
                    }
                }
                if (dir == Direction.RIGHT)
                {
                    Node n = Node.getnode(current.x + 1, current.y);
                    if (n == Target)
                    {
                        history.Add(n);
                        solutions.Add(history);
                    }
                    else
                    {
                        pathfinding(copy(history), Target, n);
                    }
                }
            }
        }

        public void generate(int x, int y)
        {
            if (Node.getnode(x, y) == null)
            {
                new Node(x, y);
            }
            if (!grid.Contains(x + ":" + y))
            {
                push(x, y);
            }
            List<Direction> directions = new List<Direction>();
            // Can we go to the left?
            if (x > 0 && !grid.Contains((x - 1) + ":" + y))
            {
                directions.Add(Direction.LEFT);
            }
            // Can we go to the right?
            if (x != width - 1 && !grid.Contains((x + 1) + ":" + y))
            {
                directions.Add(Direction.RIGHT);
            }
            // Can we go up?
            if (y > 0 && !grid.Contains(x + ":" + (y - 1)))
            {
                directions.Add(Direction.UP);
            }
            // Can we go down?
            if (y != height - 1 && !grid.Contains(x + ":" + (y + 1)))
            {
                directions.Add(Direction.DOWN);
            }
            if (directions.Count > 0) // Are there any options?
            {
                int mod = cellWidth / 2;
                // Get a random direction from the list
                Direction dir = directions[r.Next(directions.Count)];
                if (dir == Direction.LEFT)
                {
                    Node n = Node.getnode(x, y);
                    n.directions.Add(Direction.LEFT);
                    g.DrawLine(eraser, (x) * cellWidth, y * cellWidth, (x) * cellWidth, (y + 1) * cellWidth);
                    //g.DrawLine(test, x * cellWidth + mod, y * cellWidth + mod, (x - 1) * cellWidth + mod, y * cellWidth + mod);
                    generate(x - 1, y);
                }
                if (dir == Direction.RIGHT)
                {
                    Node n = Node.getnode(x, y);
                    n.directions.Add(Direction.RIGHT);
                    g.DrawLine(eraser, (x + 1) * cellWidth, y * cellWidth, (x + 1) * cellWidth, (y + 1) * cellWidth);
                    //g.DrawLine(test, x * cellWidth + mod, y * cellWidth + mod, (x + 1) * cellWidth + mod, y * cellWidth + mod);
                    generate(x + 1, y);
                }
                if (dir == Direction.UP)
                {
                    Node n = Node.getnode(x, y);
                    n.directions.Add(Direction.UP);
                    g.DrawLine(eraser, (x) * cellWidth, (y) * cellWidth, (x + 1) * cellWidth, (y) * cellWidth);
                    //g.DrawLine(test, x * cellWidth + mod, y * cellWidth + mod, x * cellWidth + mod, (y - 1) * cellWidth + mod);
                    generate(x, y - 1);
                }
                if (dir == Direction.DOWN)
                {
                    Node n = Node.getnode(x, y);
                    n.directions.Add(Direction.DOWN);
                    g.DrawLine(eraser, (x) * cellWidth, (y + 1) * cellWidth, (x + 1) * cellWidth, (y + 1) * cellWidth);
                    //g.DrawLine(test, x * cellWidth + mod, y * cellWidth + mod, x * cellWidth + mod, (y + 1) * cellWidth + mod);
                    generate(x, y + 1);
                }

            }
            else
            {
                // Backtrace your steps until there are options
                string before = pop();
                if (before == "")
                {
                    // Everything is done, Maze has been generated
                    return;
                }
                generate(Int32.Parse(before.Split(':')[0]), Int32.Parse(before.Split(':')[1]));
                return;
            }

        }

        public void push(int x, int y)
        {
            //push the int onto the stack
            stack.Add(x + ":" + y);
            grid.Add(x + ":" + y);

        }

        public string pop()
        {
            if (stack.Count > 0)
            {
                String s = stack[stack.Count - 1];
                stack.Remove(s);
                return s;
            }
            return "";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // Generate Maze
            cellWidth = Int32.Parse(textBox3.Text);
            creategrid(Int32.Parse(textBox1.Text), Int32.Parse(textBox2.Text));
        }

        public void showsolution()
        {
            int lowest = 999999;
            foreach (List<Node> s in solutions)
            {
                if (s.Count < lowest)
                {
                    lowest = s.Count;
                }
            }
            foreach (List<Node> s in solutions)
            {
                if (s.Count == lowest)
                {
                    foreach (Node n in s)
                    {
                        Thread.Sleep(1);
                        displaynode(n);
                    }
                    return;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Node> list = new List<Node>();
            Console.WriteLine("Starting PathFinding");
            pathfinding(list, Node.getnode(width - 1, height - 1), Node.getnode(0, 0));
            foreach (List<Node> nodes in solutions)
            {
                Console.WriteLine(nodes.Count);
            }
            showsolution();
            Console.WriteLine("Pathfinding Complete");
        }
    }
}
