using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public abstract class ObjectNode
{
    public Rect nodeRect;
    public string Title;
}

public abstract class BaseTreeNode : ObjectNode
{
    public void AddChild(BaseTreeNode child)
    {
        if (child == null) return;    
        if (childNodes == null)
            childNodes = new List<BaseTreeNode>();
        childNodes.Add(child);
        child.parentNode = this;
    }

    public void AddParent(BaseTreeNode parent)
    {
        if (parent != null)
        {
            if (parent.childNodes == null)
                parent.childNodes = new List<BaseTreeNode>();
            parent.childNodes.Add(this);
        }
        this.parentNode = parent;
    }

    public interface INodeContent
    {
        int GetContentType();
    }

    public BaseTreeNode parentNode;
    public List<BaseTreeNode> childNodes;
    public INodeContent content;
    //当父节点是一个条件节点(多结果)的时候.这个值存储着对应条件节点的输出
    public int selectionValue;
}