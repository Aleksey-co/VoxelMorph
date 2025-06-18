using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMover : MonoBehaviour
{
    public static Transform targetObject;

    public static Vector3 VoxelOffset;

    public static Camera camVM;
    public static Transform selectedAxis = null;
    public static Vector3 dragStartPos;
    public static Vector3 moveAxis;
    public static bool clickedVM;
    public static Vector3 forbiddenAxis;
    public static Vector3 savedHitVoxel;

    void Start()
    {
        camVM = Camera.main;
        targetObject = GameObject.Find("TargetObject").transform;
        VoxelOffset = new Vector3(0,0,0);
    }



    public static void HandleInput(Vector3 hitVoxel, Vector3 preRayVoxel, bool canBeMoved)
    {
        if (Input.GetMouseButtonDown(0) && canBeMoved)
        {
            VoxelOffset = -hitVoxel + new Vector3(127,127,127); //осталось только сделать чтобы при отпуске мыши модель перестраивалась на новом месте
            forbiddenAxis = hitVoxel - preRayVoxel;

                    dragStartPos = GetMouseWorldPosOnPlane((targetObject.position-VoxelOffset), Vector3.zero);//targetObject.position //GetAxisDirection(selectedAxis)
                    if(dragStartPos.x > 0 || dragStartPos.y > 0 || dragStartPos.z > 0){
                        dragStartPos = new Vector3(Mathf.Ceil(dragStartPos.x), Mathf.Ceil(dragStartPos.y), Mathf.Ceil(dragStartPos.z));
                    }else{
                        dragStartPos = new Vector3(Mathf.Floor(dragStartPos.x), Mathf.Floor(dragStartPos.y), Mathf.Floor(dragStartPos.z));
                    }
                    clickedVM = true;
        }
        else if (Input.GetMouseButton(0) && clickedVM && (moveAxis == null || moveAxis == Vector3.zero))
        {
            Vector3 currentPos = GetMouseWorldPosOnPlane((targetObject.position-VoxelOffset), Camera.main.transform.forward);

            if(currentPos.x > 0 || currentPos.y > 0 || currentPos.z > 0){
                currentPos = new Vector3(Mathf.Ceil(currentPos.x), Mathf.Ceil(currentPos.y), Mathf.Ceil(currentPos.z));
            }else{
                currentPos = new Vector3(Mathf.Floor(currentPos.x), Mathf.Floor(currentPos.y), Mathf.Floor(currentPos.z));
            }


            if(currentPos.x - dragStartPos.x != 0 && forbiddenAxis.x == 0){//forbiddenAxis ??? Ебаные края куба багаются потому что это тот же воксель
                moveAxis = Vector3.right;
            }else if(currentPos.y - dragStartPos.y != 0 && forbiddenAxis.y == 0){
                moveAxis = Vector3.up;
            }else if(currentPos.z - dragStartPos.z != 0 && forbiddenAxis.z == 0){
                moveAxis = Vector3.forward;
            }
        }
        else if (Input.GetMouseButton(0) && clickedVM && moveAxis != Vector3.zero)
        {
            Vector3 currentPos = GetMouseWorldPosOnPlane((targetObject.position-VoxelOffset), Camera.main.transform.forward);

            if(currentPos.x > 0 || currentPos.y > 0 || currentPos.z > 0){
                currentPos = new Vector3(Mathf.Ceil(currentPos.x), Mathf.Ceil(currentPos.y), Mathf.Ceil(currentPos.z));
            }else{
                currentPos = new Vector3(Mathf.Floor(currentPos.x), Mathf.Floor(currentPos.y), Mathf.Floor(currentPos.z));
            }

            Vector3 delta = currentPos - dragStartPos;
            float moveAmount = Vector3.Dot(delta, moveAxis);

            Vector3 buff = moveAxis * moveAmount;
            if(buff.x > 0 || buff.y > 0 || buff.z > 0){
                buff = new Vector3(Mathf.Ceil(buff.x), Mathf.Ceil(buff.y), Mathf.Ceil(buff.z));
            }else{
                buff = new Vector3(Mathf.Floor(buff.x), Mathf.Floor(buff.y), Mathf.Floor(buff.z));
            }
            
            if(buff != Vector3.zero){
                Vector3 buff2 = targetObject.localPosition + buff;

                int n = OBJConverter.n;
                if(buff2.x < (n+1) && buff2.y < (n+1) && buff2.z < (n+1) && buff2.x > -(n+1) && buff2.y > -(n+1) && buff2.z > -(n+1)){
                    targetObject.position += buff;
                    dragStartPos = currentPos;
                }
                
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            clickedVM = false;
            moveAxis = Vector3.zero;
        }
    }



    public static Vector3 GetMouseWorldPosOnPlane(Vector3 pointOnPlane, Vector3 planeNormal)
    {
        Plane plane = new Plane(planeNormal, pointOnPlane);
        Ray ray = camVM.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            return hitPoint;
        }

        return pointOnPlane;
    }


    
}
