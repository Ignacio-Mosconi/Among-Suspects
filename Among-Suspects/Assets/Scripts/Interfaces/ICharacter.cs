using UnityEngine;

public interface ICharacter
{
    CharacterName GetCharacterName();
    Sprite GetSprite(CharacterEmotion characterEmotion);
}