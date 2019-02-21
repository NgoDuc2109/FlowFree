using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace linkDot
{
    public class TouchDots : MonoBehaviour
    {
        public int tx, ty;
        // Use this for initialization
        int id;
        void Start()
        {

            id = ty * GameData.bsize + tx; // id tức vị trí của cell trên map
        }
        bool colorPath = false;
        /// <summary>
        /// bắt sự kiện khi chạm vào cell
        /// </summary>
        void OnMouseDown()
        {
            if (GameData.instance.isPause == true)
            {
                return;
            }
            if (GameData.instance.isWrong == true)
            {
                return;
            }
            if (GameData.instance.isWin) //nếu wingame thì return
                return;
            if (GameData.instance.isHolding) // nếu đang di thì return
                return;
            Transform dotChild = transform.Find("dot");

            int tdotColor = GameData.getInstance().DotColorData[id]; // lấy ra màu tương ứng với dot trên cell
            if (tdotColor != 0)
            {//bắt sự kiện khi chạm vào cell , nếu trên cell có color (tức dotColor != 0)
                clearOldPath(tdotColor); // clear hết đường vẽ cũ tương ứng với màu dotColor

                GameData.instance.pickColor = tdotColor;// gán giá trị màu cho biến pickColor
                Color tcolor = GameData.instance.colors[tdotColor];// lấy ra màu dựa vào chỉ số trong mảng màu colors
                tcolor.a = .5f;
                if (colorPath)
                {
                    transform.GetComponent<SpriteRenderer>().color = tcolor;
                }
                GameData.instance.ColorData[id] = tdotColor; // gán giá trị màu cho phần tử ColorData có id tương ứng

                GameData.instance.paths[tdotColor] = new List<int>();//ghi đè lên path cũ
                addPath(tdotColor, id);//function thêm cell tương ứng với màu
            }
            else
            {// bắt sự kiện khi chạm vào cell mà trên cell không có dot
                int cBlockColor = GameData.instance.ColorData[id];
                if (cBlockColor == 0)
                {//tap on a empty block,nothing should happen
                    Debug.Log("cell này không có dot");
                    return;
                }
                else
                {
                    //continue drawing on an already path block
                    GameData.instance.pickColor = cBlockColor;
                    //reopen the linkage
                    GameData.instance.linkedLines[cBlockColor] = 0;
                    for (int i = GameData.instance.paths[cBlockColor].Count - 1; i > 0; i--)
                    {
                        int oldid = GameData.instance.paths[cBlockColor][i];
                        if (oldid != id)
                        {//remove all paths until find the start one
                         //and clear the useless path
                            int oldx = Mathf.FloorToInt(oldid % GameData.bsize);
                            int oldy = Mathf.FloorToInt(oldid / GameData.bsize);

                            //GameObject oldBg = GameObject.Find("bg" + oldx + "_" + oldy);

                            //Color tcolor = GameData.instance.colors[0];
                            //tcolor.a = 1f;
                            //oldBg.transform.GetComponent<SpriteRenderer>().color = tcolor;
                            GameData.instance.ColorData[oldid] = 0;

                            removePath(cBlockColor, i);
                            //addPath(cBlockColor, id);
                        }
                        else
                        {
                            break;
                        }

                    }

                }
            }

            GameData.instance.isHolding = true;
            GameData.instance.startId = id;
            GameData.instance.lasttx = tx;
            GameData.instance.lastty = ty;

            //checkWin();

        }

        /// <summary>
        /// clear path cũ
        /// </summary>
        /// <param name="tcolorid">id màu của path cần clear</param>
		void clearOldPath(int tcolorid)
        {
            int tlen = GameData.instance.paths[tcolorid].Count;

            while (GameData.instance.paths[tcolorid].Count > 0)
            {
                int oldid = GameData.instance.paths[tcolorid][GameData.instance.paths[tcolorid].Count - 1];

                //and clear the old path
                int oldx = Mathf.FloorToInt(oldid % GameData.bsize);
                int oldy = Mathf.FloorToInt(oldid / GameData.bsize);

                //GameObject oldBg = GameObject.Find("bg" + oldx + "_" + oldy);

                //Color tcolor = GameData.instance.colors[0];
                //oldBg.transform.GetComponent<SpriteRenderer>().color = tcolor;
                GameData.instance.ColorData[oldid] = 0;
                removePath(tcolorid, GameData.instance.paths[tcolorid].Count - 1);
                //reopen the linkage
                GameData.instance.linkedLines[tcolorid] = 0;
            }
        }



        void OnMouseOver()
        {
            if (GameData.instance.isWin)
                return;
            if (GameData.instance.isHolding)
            {
                if (GameData.instance.pickColor != 0)
                {
                    //if dot here,get dot color
                    Transform dotChild = transform.Find("dot");
                    int tdotColor = GameData.instance.DotColorData[id]; // lấy ra chỉ số màu của dot
                    //current block color;
                    int tColorid = GameData.instance.ColorData[id]; // lấy ra chỉ số màu của cell                  
                    if ((tdotColor == 0 && tColorid == 0) || GameData.instance.pickColor == tdotColor && GameData.instance.paths[tdotColor][0] != id) // 
                    {//all places which can be draw and have not draw on something
                        //exclude not nearby blocks
                        if ((Mathf.Abs(tx - GameData.instance.lasttx) == 1 && ty == GameData.instance.lastty) ||
                            (Mathf.Abs(ty - GameData.instance.lastty) == 1 && tx == GameData.instance.lasttx))
                        {
                            addColor();
                        }
                    }
                    else
                    {//draw on an already exist self color path
                        if (tColorid != 0)
                        {
                            int tlen = GameData.instance.paths[tColorid].Count;


                            //exclude not nearby blocks
                            if ((Mathf.Abs(tx - GameData.instance.lasttx) == 1 && ty == GameData.instance.lastty) ||
                                (Mathf.Abs(ty - GameData.instance.lastty) == 1 && tx == GameData.instance.lasttx))
                            {
                                if (tColorid == GameData.instance.pickColor)
                                {//draw on self old blocks

                                    //and clear the old path
                                    int oldId = GameData.instance.paths[tColorid][GameData.instance.paths[tColorid].Count - 1];
                                    while (oldId != id)
                                    {
                                        int oldx = Mathf.FloorToInt(oldId % GameData.bsize);
                                        int oldy = Mathf.FloorToInt(oldId / GameData.bsize);
                                        //GameObject oldBg = GameObject.Find("bg" + oldx + "_" + oldy);

                                        //Color tcolor = GameData.instance.colors[0];
                                        //oldBg.transform.GetComponent<SpriteRenderer>().color = tcolor;
                                        removePath(tColorid, GameData.instance.paths[tColorid].Count - 1);
                                        GameData.instance.ColorData[oldId] = 0;

                                        //next prev id
                                        oldId = GameData.instance.paths[tColorid][GameData.instance.paths[tColorid].Count - 1];


                                        GameData.instance.lasttx = tx;
                                        GameData.instance.lastty = ty;


                                    }

                                }
                                else
                                {//draw on other block lines,cut them
                                 //clear the being cutted other color old paths
                                    int tOtherColorId = GameData.instance.ColorData[id];
                                    int oldId = GameData.instance.paths[tOtherColorId][GameData.instance.paths[tOtherColorId].Count - 1];

                                    if (GameData.instance.DotColorData[oldId] == 0 || tOtherColorId != GameData.instance.pickColor)
                                    {//if this place doesnt have a dot or this place is a different color
                                        if (GameData.instance.DotColorData[id] == 0)
                                        {   //make wont draw on other color dots
                                            while (oldId != id)
                                            {

                                                int oldx = Mathf.FloorToInt(oldId % GameData.bsize);
                                                int oldy = Mathf.FloorToInt(oldId / GameData.bsize);
                                                //GameObject oldBg = GameObject.Find("bg" + oldx + "_" + oldy);

                                                //Color tcolor = GameData.instance.colors[0];
                                                //oldBg.transform.GetComponent<SpriteRenderer>().color = tcolor;
                                                //												GameData.instance.paths [tOtherColorId].RemoveAt (GameData.instance.paths [tOtherColorId].Count - 1);
                                                removePath(tOtherColorId, GameData.instance.paths[tOtherColorId].Count - 1);
                                                GameData.instance.ColorData[oldId] = 0;

                                                //next prev id
                                                oldId = GameData.instance.paths[tOtherColorId][GameData.instance.paths[tOtherColorId].Count - 1];

                                            }

                                            if (oldId == id)
                                            {
                                                int oldx = Mathf.FloorToInt(oldId % GameData.bsize);
                                                int oldy = Mathf.FloorToInt(oldId / GameData.bsize);
                                                //GameObject oldBg = GameObject.Find("bg" + oldx + "_" + oldy);

                                                //Color tcolor = GameData.instance.colors[0];
                                                //oldBg.transform.GetComponent<SpriteRenderer>().color = tcolor;
                                                //												GameData.instance.paths [tOtherColorId].RemoveAt (GameData.instance.paths [tOtherColorId].Count - 1);
                                                removePath(tOtherColorId, GameData.instance.paths[tOtherColorId].Count - 1);
                                                GameData.instance.ColorData[oldId] = 0;


                                            }
                                        }
                                    }




                                }
                            }
                        }
                    }

                }
                CheckWin();
            }
        }



        void addColor()
        {
            int tdotColor = GameData.instance.DotColorData[id];
            if ((tdotColor != 0 && tdotColor != GameData.instance.pickColor)//draw on other color dots(not block)

            )
            {
                GameData.instance.isHolding = false;
                GameData.instance.pickColor = 0;
                return;
            }




            Color tcolor = GameData.instance.colors[GameData.instance.pickColor];
            tcolor.a = .5f;
            if (colorPath)
            {
                transform.GetComponent<SpriteRenderer>().color = tcolor;
            }
            //			GameData.instance.paths[GameData.instance.pickColor].Add (id);
            addPath(GameData.instance.pickColor, id);
            //			print ("added"+GameData.instance.pickColor);

            GameData.instance.ColorData[id] = GameData.instance.pickColor;//write color to data


            if (tdotColor != 0 && tdotColor == GameData.instance.pickColor && GameData.instance.paths[tdotColor].Count > 1)
            {
                GameData.instance.linkedLines[tdotColor] = 1;
                GameData.instance.pickColor = 0;
            }


            GameData.instance.lasttx = tx;
            GameData.instance.lastty = ty;
        }


        void OnMouseUp()
        {
            if (GameData.instance.isWin)
                return;
            GameData.instance.isHolding = false;


        }


        static bool canPlatDotSfx = true;//make draw sound effect not be too frequent;
        IEnumerator sfxGap()
        {
            yield return new WaitForSeconds(.2f);
            canPlatDotSfx = true;
        }
        void addPath(int colorId, int placeId)
        {

            if (canPlatDotSfx)
            {
                string tsfx = "d" + Mathf.FloorToInt(Random.Range(0, 6));
                //GameManager.getInstance().playSfx(tsfx);
                AudioManager.Instance.PlayEffectClip();
                canPlatDotSfx = false;
                StartCoroutine("sfxGap");
            }

            GameData.instance.paths[colorId].Add(placeId);// add thêm cell vào list path của màu hiện tại
            int tlen = GameData.instance.paths[colorId].Count; // số cell của list path của màu hiện tại
            if (tlen > 1)
            {
                // lastId1 và lastId2 là 2 phần tử cuối cùng của list path tương ứng với màu colorId
                int tlastId1 = GameData.instance.paths[colorId][tlen - 2];
                int tlastId2 = GameData.instance.paths[colorId][tlen - 1];// phần tử cuối cùng

                int tx = Mathf.FloorToInt(tlastId1 % GameData.bsize);
                int ty = Mathf.FloorToInt(tlastId1 / GameData.bsize);

                int placeOffset = tlastId2 - tlastId1; // offset giữa 2 phần tử , có 4 trường hợp
                int tRight = 1;
                int tLeft = -1;
                int tUp = GameData.bsize;
                int tDown = -GameData.bsize;
                GameObject tlink = null;
                if (placeOffset == tRight) // phần tử thứ 2 nằm bên phải phần tử thứ 1
                {//right
                    tlink = GameObject.Find("linkr" + tx + "_" + ty);
                }
                else if (placeOffset == tLeft) // phần tử thứ 2 nằm bên trái phần tử thứ 1
                {
                    tlink = GameObject.Find("linkl" + tx + "_" + ty);
                }
                else if (placeOffset == tUp) // phần tử thứ 2 nằm bên trên phần tử thứ 1
                {
                    tlink = GameObject.Find("linku" + tx + "_" + ty);
                }
                else if (placeOffset == tDown) // phần tử thứ 2 nằm bên dưới phần tử thứ 1
                {
                    tlink = GameObject.Find("linkd" + tx + "_" + ty);
                }
                if (tlink != null)
                {
                    tlink.GetComponent<SpriteRenderer>().color = GameData.instance.colors[colorId];
                    Vector3 currentScale = tlink.transform.localScale;
                    tlink.transform.localScale = new Vector3(0, tlink.transform.localScale.y, tlink.transform.localScale.z);
                    tlink.transform.DOScaleX(currentScale.x, 0.1f);
                }
            }

        }

        void removePath(int colorId, int index)
        {
            //clear all linkage on this node
            int tlastId = GameData.instance.paths[colorId][index];
            int tx = Mathf.FloorToInt(tlastId % GameData.bsize);
            int ty = Mathf.FloorToInt(tlastId / GameData.bsize);
            GameObject tlink = null;
            tlink = GameObject.Find("linkr" + tx + "_" + ty);
            tlink.GetComponent<SpriteRenderer>().color = GameData.instance.colors[0];
            tlink = GameObject.Find("linkl" + tx + "_" + ty);
            tlink.GetComponent<SpriteRenderer>().color = GameData.instance.colors[0];
            tlink = GameObject.Find("linku" + tx + "_" + ty);
            tlink.GetComponent<SpriteRenderer>().color = GameData.instance.colors[0];
            tlink = GameObject.Find("linkd" + tx + "_" + ty);
            tlink.GetComponent<SpriteRenderer>().color = GameData.instance.colors[0];


            GameData.instance.paths[colorId].RemoveAt(index);

            if (index > 0)
            {
                tlastId = GameData.instance.paths[colorId][index - 1];
                tx = Mathf.FloorToInt(tlastId % GameData.bsize);
                ty = Mathf.FloorToInt(tlastId / GameData.bsize);

                tlink = GameObject.Find("linkr" + tx + "_" + ty);
                tlink.GetComponent<SpriteRenderer>().color = GameData.instance.colors[0];
                tlink = GameObject.Find("linkl" + tx + "_" + ty);
                tlink.GetComponent<SpriteRenderer>().color = GameData.instance.colors[0];
                tlink = GameObject.Find("linku" + tx + "_" + ty);
                tlink.GetComponent<SpriteRenderer>().color = GameData.instance.colors[0];
                tlink = GameObject.Find("linkd" + tx + "_" + ty);
                tlink.GetComponent<SpriteRenderer>().color = GameData.instance.colors[0];
            }


            GameData.instance.linkedLines[colorId] = 0;//reopen the linkage
        }

        void CheckWin()
        {
            int nwin = 0;
            for (int k = 0; k < GameData.instance.linkedLines.Length; k++)
            {
                if (GameData.instance.linkedLines[k] == 1)
                {
                    nwin++;
                }

            }

            if (nwin >= GameData.instance.winLinkCount)
            {//enough linkage
                GameObject reciever = GameObject.Find("all_game");
                for (int i = 0; i < GameData.instance.ColorData.Length; i++)
                {
                    if (GameData.instance.ColorData[i] == 0)
                    {
                        GameData.instance.isWrong = true;
                        if (reciever != null)
                        {
                            reciever.transform.parent.GetComponent<MainScript>().ShowWrongPanel();
                        }
                        return;
                    }
                }
                GameData.instance.isHolding = false;
                GameData.instance.isWin = true;
                if (reciever != null)
                {
                    reciever.transform.parent.GetComponent<MainScript>().gameWin();
                    print("game win!!");
                }
            }
        }
    }
}
