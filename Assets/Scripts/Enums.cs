// Действия игрока
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

// Тип ввода
public enum EInputKind
{
    Key,
    Axis,
    MouseButton
}

// Направления, в которые может смотреть или двигаться персонаж
public enum EDirection
{
    Left,
    Right
}

// Виды интерактивных объектов
public enum EActiveObjectType
{
    Pickable,
    Target,
}

// Типы подбираемых предметов
public enum EItemType
{
    Key,
    Weapon,
    Heal
}