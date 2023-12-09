using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node : MonoBehaviour {

    public bool isOccupied;
    public bool merging;
    [SerializeField] private int value;
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private TextMeshPro text;
    [SerializeField] public Node mergingNode;

   
    public void Init(Block block, Node node)
    {
        Pos = node.Pos;
        value = block.value;
        renderer.color = block.color;
        text.text = block.value.ToString();
        isOccupied = true;
        mergingNode = null;
        merging = false;
        node.value = block.value;
        node.isOccupied = true;
    }

   public void MergeNode(Node nodeToMergeWith) 
   {
       mergingNode = nodeToMergeWith;
       nodeToMergeWith.merging = true;
   }
    
   public bool CanMerge(int value)
   {
       if(value == this.value && !merging && mergingNode == null) return true;
       return false;
   }

   public int Value
   {
       get => value;
       set => this.value = value;
   }
   public Vector2 Pos
   {
       get => transform.position;
       set => transform.position = value;
   }
}
