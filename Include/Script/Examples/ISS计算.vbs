'*********************************************************************************
' ����ISSԪ�ص������Զ�ʵʱ����Ľű�
' ע�⣺����Visual Basic�ű��������Բ����ִ�Сд 
' Script Language : Visual Basic
' Author : YangMingkun, Date : 2011-11-21
' Copyright (C) Heren Health Services (SUPCON) Co.,Ltd.
'*********************************************************************************


'����Calculate�ӿں����ڱ�д���Ĵ���,�벻Ҫ�޸ĺ������������޷����༭��ϵͳʶ��
'����szElementName��Ԫ������,����ֵ���ɱ༭��ϵͳ����ʱ��̬�����
Public Overrides Sub Calculate(ByVal szElementName As String)

    Dim szName1 As String = "ISSͷ�;���"
    Dim szName2 As String = "ISS�沿"
    Dim szName3 As String = "ISS�ز�"
    Dim szName4 As String = "ISS��������ǻ"
    Dim szName5 As String = "ISS��֫�͹���"
    Dim szName6 As String = "ISS���"

    '����û���ǰ�޸ĵĲ��Ǵ˽ű���Ҫ��ص�Ԫ��,��ô�˳�,���Ǻ���Ҫ��
    '��Ϊ������˳�,�ͻ�Ӱ��༭��ϵͳ������Ч��
    If szElementName <> szName1 AndAlso szElementName <> szName2 _
        AndAlso szElementName <> szName3 AndAlso szElementName <> szName4 _
        AndAlso szElementName <> szName5 AndAlso szElementName <> szName6 Then
        Return
    End If

    Dim szElementValue As String = ""

    Dim nElementValue1 As Integer = 0
    Dim nElementValue4 As Integer = 0
    Dim nElementValue5 As Integer = 0
    Dim nElementValue2 As Integer = 0
    Dim nElementValue3 As Integer = 0
    Dim nElementValue6 As Integer = 0

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

    'ͨ��GetElementValue�ӿ�,��ȡ����ʱ����Ҫ���ĵ��е�Ԫ��ֵ(�ַ������͵�ֵ)
    Me.GetElementValue(szName4.Trim(), szElementValue)
    If Not Integer.TryParse(szElementValue, nElementValue4) Then
        If szElementName = szName4 Then
            Me.ShowElementTip("�Ƿ�����������", "������ȷ����Ԫ����ֵ!")
            Exit Sub
        End If
    End If


    Me.GetElementValue(szName5, szElementValue)
    If Not Integer.TryParse(szElementValue, nElementValue5) Then
        If szElementName = szName5 Then
            Me.ShowElementTip("�Ƿ�����������", "������ȷ����Ԫ����ֵ!")
            Exit Sub
        End If
    End If

    Me.GetElementValue(szName6, szElementValue)
    If Not Integer.TryParse(szElementValue, nElementValue6) Then
        If szElementName = szName6 Then
            Me.ShowElementTip("�Ƿ�����������", "������ȷ����Ԫ����ֵ!")
            Exit Sub
        End If
    End If


    '���к�������,Ȼ��������ͨ��SetElementValue�ӿ����õ�������ָ����Ԫ��
    Dim result As Integer = nElementValue1 + nElementValue2 + nElementValue3 + nElementValue4 + nElementValue5 + nElementValue6
    Me.SetElementValue("ISS", result.ToString())

End Sub
