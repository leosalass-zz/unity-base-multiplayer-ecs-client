using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using System.Collections.Generic;

public class PlayerCharacterEntitySpawner : MonoBehaviour
{
    private EntityManager entityManager;
    private EntityArchetype playerCharacterArchetype;
    private World defaultWorld;
    private GameObjectConversionSettings settings;

    private Entity playerCharacterEntityPrefab;
    private List<Entity> _entities;

    public PlayerCharacterEntitySpawner()
    {
        _entities = new List<Entity>();
        defaultWorld = World.DefaultGameObjectInjectionWorld;
        entityManager = defaultWorld.EntityManager;
        settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
        SetPlayerCharacter_001();
    }

    public void SetPlayerCharacter_001()
    {
        GameObject prefab = Resources.Load("Characters/PlayerCharacters/Character_001/Prefabs/PlayerCharacter_001") as GameObject;
        playerCharacterEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
    }

    public void SpawnPlayerCharacterEntity(float3 pos)
    {
        Entity entity = entityManager.Instantiate(playerCharacterEntityPrefab);
        entityManager.SetComponentData(entity, new Translation
        {
            Value = pos
        });
        entityManager.SetName(entity, "PlayercharacterEntity");
        _entities.Add(entity);

    }

    public void DestroyAndResetAllEntities()
    {
        foreach (Entity entity in _entities)
        {
            entityManager.DestroyEntity(entity);
        }

        _entities.Clear();
    }
}
