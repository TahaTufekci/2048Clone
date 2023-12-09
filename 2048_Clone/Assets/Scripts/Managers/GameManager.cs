using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    
    [SerializeField] private Node nodePrefab;
    [SerializeField] private float travelTime = 0.2f;
    [SerializeField] private int winCondition = 2048;
    private int blockNumberToSpawn;

    //[SerializeField] private AudioClip[] _moveClips;
    //[SerializeField] private AudioClip[] _matchClips;
    //[SerializeField] private AudioSource audioSource;

    [SerializeField] private List<Node> nodeControlList;
    [SerializeField] private List<Block> blockList;
    [SerializeField] private Node mergeNode;
    private List<Node> nodeList;
    private GameState currentGameState;
    public static Action<GameState> OnGameStateChanged;

    
    private void Awake()
    {
        blockNumberToSpawn = 0;
        nodeList = new List<Node>();
    }

    void Start() {
        GenerateFirstNodes();
    }

    private void ChangeGameState(GameState state) {
        currentGameState = state;

        switch (state) {
            case GameState.SpawningBlocks:
                SpawnBlocks(blockNumberToSpawn++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                AudioManager.Instance.PlaySound("CorrectSFX");
                OnGameStateChanged?.Invoke(GameState.Win);
                break;
            case GameState.Lose:
                AudioManager.Instance.PlaySound("IncorrectSFX");
                OnGameStateChanged?.Invoke(GameState.Lose);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    
    void Update() {
        
        if(currentGameState != GameState.WaitingInput) return;

        if(Input.GetKeyDown(KeyCode.LeftArrow)) Shift(Vector2.left);
        if(Input.GetKeyDown(KeyCode.RightArrow)) Shift(Vector2.right);
        if(Input.GetKeyDown(KeyCode.UpArrow)) Shift(Vector2.up);
        if(Input.GetKeyDown(KeyCode.DownArrow)) Shift(Vector2.down);
    }

    void GenerateFirstNodes() {
        ChangeGameState(GameState.SpawningBlocks);
    }

    void SpawnBlocks(int blockNo) 
    {
        List<Node> freeNodes = nodeControlList.Where(n => !n.isOccupied).OrderBy(b => Random.value).ToList();

        foreach (var node in freeNodes.Take(blockNo)) {
            SpawnBlock(node,Random.value > 0.8f ? 1 : 0);
        }

        if (freeNodes.Count() == 1) {
            ChangeGameState(GameState.Lose);
            return;
        }

        ChangeGameState(nodeList.Any(n=>n.Value == winCondition) ? GameState.Win : GameState.WaitingInput);
    }

    void SpawnBlock(Node node, int value) 
    {
        Node newNode = Instantiate(nodePrefab, node.Pos, Quaternion.identity);
        newNode.Init(blockList[value],node);
        nodeList.Add(newNode);
    }

    void Shift(Vector2 dir)
    {
        ChangeGameState(GameState.Moving);

        List<Node> orderedNodes = nodeList 
            .OrderBy(n => n.Pos.x)
            .ThenBy(n => n.Pos.y)
            .ToList();

        if (dir == Vector2.right || dir == Vector2.up)
            orderedNodes.Reverse();


        foreach (var node in orderedNodes)
        {
            var nextNode = GetControlNodeAtPosition(node.Pos);
            var possibleNode = GetControlNodeAtPosition(nextNode.Pos + dir);
            while (possibleNode != null)
            {
                mergeNode = GetNodeAtPosition(possibleNode.Pos);
                    
                    if (mergeNode && possibleNode.isOccupied && mergeNode.CanMerge(node.Value))
                    {
                        Node controlNode = GetControlNodeAtPosition(nextNode.Pos); // make the control node before the merging node default
                        TurnNodeToDefault(controlNode);
                        node.MergeNode(mergeNode);
                        break;
                    }
                    if(mergeNode && possibleNode.isOccupied && !mergeNode.CanMerge(node.Value))
                    {
                        break;
                    }

                    if (!possibleNode.isOccupied)
                    {
                        ArrangeControlNode(possibleNode,node);
                        TurnNodeToDefault(nextNode);
                        nextNode = possibleNode;
                        possibleNode = GetControlNodeAtPosition(nextNode.Pos + dir);
                    }
            }
            node.Pos = nextNode.Pos;
        }
        
        var sequence = DOTween.Sequence();

        foreach (var node in orderedNodes)
        {
            var movePoint = node.mergingNode != null ? node.mergingNode.Pos : node.Pos;

            sequence.Insert(0, node.transform.DOMove(movePoint, travelTime).SetEase(Ease.InQuad));
        }
        sequence.OnComplete(() => {
            var mergeNodes = orderedNodes.Where(b => b.mergingNode != null).ToList();
            foreach (var node in mergeNodes) {
                MergeNodes(node,node.mergingNode);
            }
            ChangeGameState(GameState.SpawningBlocks);
        });
    }

    void MergeNodes(Node baseNode, Node mergingNode) 
   {
    int index = (int)Mathf.Log(baseNode.Value,2);
    
    SpawnMergingBlock(mergingNode, index);
    
    RemoveNode(baseNode);
    RemoveNode(mergingNode);
    }
   void SpawnMergingBlock(Node node, int value) 
   {
       Node newNode = Instantiate(nodePrefab, node.Pos, Quaternion.identity);
       newNode.Init(blockList[value],node);
       nodeList.Add(newNode);
       Node controlNode = GetControlNodeAtPosition(node.Pos); // arrange the control node on the merging node
       ArrangeControlNode(controlNode,newNode);
       
       ChangeGameState(newNode.Value == winCondition ? GameState.Win : GameState.WaitingInput); // check the win condition
   }
   void ArrangeControlNode(Node node, Node newNode)
   {
       node.Value = newNode.Value;
       node.isOccupied = true;
   }
   void TurnNodeToDefault(Node node)
   {
       node.isOccupied = false;
       node.Value = 0;
   }
       
    void RemoveNode(Node node) {
        nodeList.Remove(node);
        Destroy(node.gameObject);
    }

    Node GetControlNodeAtPosition(Vector2 pos) {
        return nodeControlList.Find(n => n.Pos == pos);
    }
    Node GetNodeAtPosition(Vector2 pos) {
        return nodeList.Find(n => n.Pos == pos);
    }

}