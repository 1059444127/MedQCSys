'*********************************************************************************
' ����ϵͳ�ṩ��RTSԪ�ص������Զ�ʵʱ����Ľű�
' ע�⣺����Visual Basic�ű��������Բ����ִ�Сд 
' Script Language : Visual Basic
' Author : YangMingkun, Date : 2011-11-21
' Copyright (C) Heren Health Services (SUPCON) Co.,Ltd.
'*********************************************************************************


'����Calculate�ӿں����ڱ�д���Ĵ���,�벻Ҫ�޸ĺ������������޷����༭��ϵͳʶ��
'����szElementName��Ԫ������,����ֵ���ɱ༭��ϵͳ����ʱ��̬�����
Public Overrides Sub Calculate(ByVal szElementName As String)

    Dim szName1 As String = "RTSGCS"
    Dim szName2 As String = "RTS����ѹ"
    Dim szName3 As String = "RTS����"

    '����û���ǰ�޸ĵĲ��Ǵ˽ű���Ҫ��ص�Ԫ��,��ô�˳�,���Ǻ���Ҫ��
    '��Ϊ������˳�,�ͻ�Ӱ��༭��ϵͳ������Ч��
    If szElementName <> szName1 AndAlso szElementName <> szName2 _
       AndAlso szElementName <> szName3 Then
        Return
    End If

    Dim szElementValue As String = ""

    Dim nElementValue1 As Integer = 0
    Dim nElementValue2 As Integer = 0
    Dim nElementValue3 As Integer = 0

    'ͨ��HideElementTip�ӿڼ�ʱ���ص�֮ǰ�����Ѿ���������ʾ����
    Me.HideElementTip()

    'ͨ��GetElementValue�ӿ�,��ȡ����ʱ����Ҫ���ĵ��е�Ԫ��ֵ(�ַ������͵�ֵ)
    Me.GetElementValue(szName1.Trim(), szElementValue)
    If Not Integer.TryParse(szElementValue, nElementValue1) Then
        If szElementName = szName1 Then
            Me.ShowElementTip("�Ƿ�����������", "������ȷ����Ԫ����ֵ!")
            Exit Sub
        End If
    End If

    'ͨ��GetElementValue�ӿ�,��ȡ����ʱ����Ҫ���ĵ��е�Ԫ��ֵ(�ַ������͵�ֵ)
    Me.GetElementValue(szName2.Trim(), szElementValue)
    If Not Integer.TryParse(szElementValue, nElementValue2) Then
        If szElementName = szName2 Then
            Me.ShowElementTip("�Ƿ�����������", "������ȷ����Ԫ����ֵ!")
            Exit Sub
        End If
    End If

    'ͨ��GetElementValue�ӿ�,��ȡ����ʱ����Ҫ���ĵ��е�Ԫ��ֵ(�ַ������͵�ֵ)
    Me.GetElementValue(szName3.Trim(), szElementValue)
    If Not Integer.TryParse(szElementValue, nElementValue3) Then
        If szElementName = szName3 Then
            Me.ShowElementTip("�Ƿ�����������", "������ȷ����Ԫ����ֵ!")
            Exit Sub
        End If
    End If

    '���к�������,Ȼ��������ͨ��SetElementValue�ӿ����õ�������ָ����Ԫ��
    Dim result As Integer = nElementValue1 + nElementValue2 + nElementValue3
    Me.SetElementValue("RTS", result.ToString())

End Sub
