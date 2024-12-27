using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��������scrollview�����⻬��ʱ��buff��ϸ��Ϣ
/// xushi
/// </summary>
public class ScrollToolManager : singleton<ScrollToolManager>
{
    public List<Buff_Property> buff_Property;
    public List<Buff_Weapon> buff_Weapon;
    public List<Btn_Buff> btn_Buffs;

    //��ǰ�Ƿ����չʾbuff��ϸ��Ϣ�ı�־������scrollTool�ظ�����
    public bool canShowFlag=true;

    private void Start()
    {
        buff_Property = new List<Buff_Property>();
        buff_Weapon = new List<Buff_Weapon>();
        btn_Buffs = new List<Btn_Buff>();
    }

    //��ʼ����ǰ��������еĻ���item
    public void InitScrollabelBuffItem()
    {

        //��ȡ����ͬ������
        List<BasePanel> temp_Property = PanelManager.Instance.GetAllPanel("Buff_Property");
        foreach (var item in temp_Property)
        {
            if(!buff_Property.Contains((Buff_Property)item))
                buff_Property.Add((Buff_Property)item);
        }
        List<BasePanel> temp_Weapon = PanelManager.Instance.GetAllPanel("Buff_Weapon");
        foreach (var item in temp_Weapon)
        {
            if(!buff_Weapon.Contains((Buff_Weapon)item))
                buff_Weapon.Add((Buff_Weapon)item);
        }
        List<BasePanel> temp_buff = PanelManager.Instance.GetAllPanel("Btn_Buff");
        foreach (var item in temp_buff)
        {
            if (!btn_Buffs.Contains((Btn_Buff)item))
                btn_Buffs.Add((Btn_Buff)item);
        }
    }
    //��ֹ����buff��ϸ��Ϣչʾ
    public void DisableShow()
    {
        if (canShowFlag == true)
        {
            canShowFlag= false;

            foreach (var item in buff_Property)
            {
                item.CloseInfo();
                item.SwitchShow(false);
            }
            foreach (var item in buff_Weapon)
            {
                item.CloseInfo();
                item.SwitchShow(false);
            }
            foreach (var item in btn_Buffs)
            {
                item.CloseInfo();
                item.SwitchShow(false);
            }
        }
        
    }
    //������buff��ϸ��Ϣչʾ
    public void EnableShow()
    {
        if (canShowFlag == false)
        {
            canShowFlag = true;

            foreach (var item in buff_Property)
            {
                item.SwitchShow(true);
            }
            foreach (var item in buff_Weapon)
            {
                item.SwitchShow(true);
            }
            foreach (var item in btn_Buffs)
            {
                item.SwitchShow(true);
            }
        }
        
    }
}
