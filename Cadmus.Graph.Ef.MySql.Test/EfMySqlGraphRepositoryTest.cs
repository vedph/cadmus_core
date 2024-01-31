using Cadmus.Graph.Ef.Test;
using Fusi.DbManager;
using Fusi.DbManager.MySql;
using Xunit;

namespace Cadmus.Graph.Ef.MySql.Test;

[Collection(nameof(NonParallelResourceCollection))]
public sealed class EfMySqlGraphRepositoryTest : EfGraphRepositoryTest
{
    private IDbManager? _manager;

    public override string ConnectionStringTemplate =>
    "Server=localhost;Database={0};Uid=root;Pwd=mysql;";

    public override IDbManager DbManager =>
        _manager ??= new MySqlDbManager(ConnectionStringTemplate);

    protected override IGraphRepository GetRepository()
    {
        EfMySqlGraphRepository repository = new();
        repository.Configure(new EfGraphRepositoryOptions
        {
            ConnectionString = ConnectionString
        });
        return repository;
    }

    protected override string GetSchema() => EfMySqlGraphRepository.GetSchema();

#pragma warning disable S2699 // Tests should include assertions

    #region Namespace
    [Fact]
    public void GetNamespaces_Ok() => DoGetNamespaces_Ok();

    [Fact]
    public void GetNamespaces_Prefix_Ok() => DoGetNamespaces_Prefix_Ok();

    [Fact]
    public void GetNamespaces_Uri_Ok() => DoGetNamespaces_Uri_Ok();

    [Fact]
    public void LookupNamespace_NotExisting_Null()
        => DoLookupNamespace_NotExisting_Null();

    [Fact]
    public void LookupNamespace_Existing_Ok()
        => DoLookupNamespace_Existing_Ok();

    [Fact]
    public void DeleteNamespaceByPrefix_NotExisting_Nope()
        => DoDeleteNamespaceByPrefix_NotExisting_Nope();

    [Fact]
    public void DeleteNamespaceByPrefix_Existing_Ok()
        => DoDeleteNamespaceByPrefix_Existing_Ok();

    [Fact]
    public void DeleteNamespaceByUri_NotExisting_Nope()
        => DoDeleteNamespaceByUri_NotExisting_Nope();

    [Fact]
    public void DeleteNamespaceByUri_Existing_Ok()
        => DoDeleteNamespaceByUri_Existing_Ok();
    #endregion

    #region Uid
    [Fact]
    public void AddUid_NoClash_AddedNoSuffix()
        => DoAddUid_NoClash_AddedNoSuffix();

    [Fact]
    public void AddUid_ClashUnique_AddedWithSuffix()
        => DoAddUid_ClashUnique_AddedWithSuffix();

    [Fact]
    public void AddUid_ClashNotUnique_ReusedWithSuffix()
        => DoAddUid_ClashNotUnique_ReusedWithSuffix();
    #endregion

    #region Uri
    [Fact]
    public void AddUri_NotExisting_Added()
        => DoAddUri_NotExisting_Added();

    [Fact]
    public void AddUri_Existing_Nope()
        => DoAddUri_NotExisting_Added();
    #endregion

    #region Node
    [Fact]
    public void GetNodes_NoFilter_Ok() => DoGetNodes_NoFilter_Ok();

    [Fact]
    public void GetNodes_NoFilterPage2_Ok() => DoGetNodes_NoFilterPage2_Ok();

    [Fact]
    public void GetNodes_ByLabel_Ok() => DoGetNodes_ByLabel_Ok();

    [Fact]
    public void GetNodes_ByLabelAndClass_Ok() => DoGetNodes_ByLabelAndClass_Ok();

    [Fact]
    public void GetNodes_BySourceType_Ok() => DoGetNodes_BySourceType_Ok();

    [Fact]
    public void GetNodes_BySidExact_Ok() => DoGetNodes_BySidExact_Ok();

    [Fact]
    public void GetNodes_BySidPrefix_Ok() => DoGetNodes_BySidPrefix_Ok();

    [Fact]
    public void GetNodes_ByNoTag_Ok() => DoGetNodes_ByNoTag_Ok();

    [Fact]
    public void GetNodes_ByTag_Ok() => DoGetNodes_ByTag_Ok();

    [Fact]
    public void GetNodes_ByLinkedNode_Ok() => DoGetNodes_ByLinkedNode_Ok();

    [Fact]
    public void GetNodes_ByLinkedNodeWithMatchingRole_Ok()
        => DoGetNodes_ByLinkedNodeWithMatchingRole_Ok();

    [Fact]
    public void GetNodes_ByLinkedNodeWithNonMatchingRole_Ok()
        => DoGetNodes_ByLinkedNodeWithNonMatchingRole_Ok();

    [Fact]
    public void DeleteNode_NotExisting_Nope()
        => DoDeleteNode_NotExisting_Nope();

    [Fact]
    public void DeleteNode_Existing_Ok() => DoDeleteNode_Existing_Ok();

    [Fact]
    public void GetLinkedNodes_Ok() => DoGetLinkedNodes_Ok();
    #endregion

    #region Property
    [Fact]
    public void GetProperties_NoFilter_Ok() => DoGetProperties_NoFilter_Ok();

    [Fact]
    public void GetProperties_ByUid_Ok() => DoGetProperties_ByUid_Ok();

    [Fact]
    public void GetProperties_ByDataType_Ok()
        => DoGetProperties_ByDataType_Ok();

    [Fact]
    public void GetProperties_ByLiteralEditor_Ok()
        => DoGetProperties_ByLiteralEditor_Ok();

    [Fact]
    public void GetProperty_NotExisting_Null()
        => DoGetProperty_NotExisting_Null();

    [Fact]
    public void GetProperty_Existing_NotNull()
        => DoGetProperty_Existing_NotNull();

    [Fact]
    public void GetPropertyByUri_NotExisting_Null()
        => DoGetPropertyByUri_NotExisting_Null();

    [Fact]
    public void GetPropertyByUri_Existing_Ok()
        => DoGetPropertyByUri_Existing_Ok();

    [Fact]
    public void DeleteProperty_NotExisting_Nope()
        => DoDeleteProperty_NotExisting_Nope();

    [Fact]
    public void DeleteProperty_Existing_Ok()
        => DoDeleteProperty_Existing_Ok();
    #endregion

    #region Triple
    [Fact]
    public void AddTriple_NotExisting_Added() => DoAddTriple_NotExisting_Added();

    [Fact]
    public void AddTriple_Same_Unchanged() => DoAddTriple_Same_Unchanged();

    [Fact]
    public void GetTriples_NoFilter_Ok() => DoGetTriples_NoFilter_Ok();

    [Fact]
    public void GetTriples_BySubjectId_Ok() => DoGetTriples_BySubjectId_Ok();

    [Fact]
    public void GetTriples_ByPredicateId_Ok() => DoGetTriples_ByPredicateId_Ok();

    [Fact]
    public void GetTriples_ByObjectId_Ok() => DoGetTriples_ByObjectId_Ok();

    [Fact]
    public void GetTriples_ByLiteral_Ok() => DoGetTriples_ByLiteral_Ok();

    [Fact]
    public void GetTriples_BySidExact_Ok() => DoGetTriples_BySidExact_Ok();

    [Fact]
    public void GetTriples_BySidPrefix_Ok() => DoGetTriples_BySidPrefix_Ok();

    [Fact]
    public void GetTriples_ByNoTag_Ok() => DoGetTriples_ByNoTag_Ok();

    [Fact]
    public void GetTriples_ByTag_Ok() => DoGetTriples_ByTag_Ok();

    [Fact]
    public void GetTriple_NotExisting_Null() => DoGetTriple_NotExisting_Null();

    [Fact]
    public void GetTriple_Existing_Ok() => DoGetTriple_Existing_Ok();

    [Fact]
    public void DeleteTriple_NotExisting_Nope() => DoDeleteTriple_NotExisting_Nope();

    [Fact]
    public void DeleteTriple_Existing_Ok() => DoDeleteTriple_Existing_Ok();

    [Fact]
    public void GetTripleGroups_Subject_Ok() => DoGetTripleGroups_Ok();
    #endregion

    #region Thesaurus
    [Theory]
    [InlineData(null)]
    [InlineData("x:classes/")]
    public void AddThesaurus_Root_Ok(string? prefix) => DoAddThesaurus_Root_Ok(prefix);

    [Fact]
    public void AddRealThesaurus_Ok() => DoAddRealThesaurus_Ok();
    #endregion

    #region Mapping
    [Fact]
    public void LoadMappings_Ok() => DoLoadMappings_Ok();

    [Fact]
    public void AddMapping_NotExisting_Ok() => DoAddMapping_NotExisting_Ok();

    [Fact]
    public void AddMapping_Existing_Ok() => DoAddMapping_Existing_Ok();

    [Fact]
    public void AddMappingByName_NotExisting_Ok() => DoAddMappingByName_NotExisting_Ok();

    [Fact]
    public void AddMappingByName_Existing_Ok() => DoAddMappingByName_Existing_Ok();

    [Fact]
    public void DeleteMapping_NotExisting_Nope() => DoDeleteMapping_NotExisting_Nope();

    [Fact]
    public void DeleteMapping_Existing_Ok() => DoDeleteMapping_Existing_Ok();

    [Fact]
    public void GetMappings_Ok() => DoGetMappings_Ok();

    [Fact]
    public void FindMappings_Ok() => DoFindMappings_Ok();

    [Fact]
    public void UpdateGraph_Event_Ok() => DoUpdateGraph_Event_Ok();

    [Fact]
    public void UpdateGraph_Work_Ok() => DoUpdateGraph_Work_Ok();

    [Fact]
    public void UpdateGraph_RelatedWork_Ok() => DoUpdateGraph_RelatedWork_Ok();

    [Fact]
    public void UpdateGraph_RelatedWorkUpdate_Ok()
        => DoUpdateGraph_RelatedWorkUpdate_Ok();

    [Fact]
    public void UpdateGraph_Delete_Ok() => DoUpdateGraph_Delete_Ok();
    #endregion

#pragma warning restore S2699 // Tests should include assertions
}
