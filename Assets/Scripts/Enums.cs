// �������� ������
public enum EPlayerActions
{
    Stand,
    MoveLeft,
    MoveRight,
    Interact,
    OpenInventory,
    Use,
    Delete,
    SaveInventory,
    LoadInventory,
    Sort,
}

// ��� �����
public enum EInputKind
{
    Key,
    Axis,
    MouseButton
}

// �����������, � ������� ����� �������� ��� ��������� ��������
public enum EDirection
{
    Left,
    Right
}

// ���� ������������� ��������
public enum EActiveObjectType
{
    Pickable,
    Target,
}

// ���� ����������� ���������
public enum EItemType
{
    Key,
    Weapon,
    Heal
}