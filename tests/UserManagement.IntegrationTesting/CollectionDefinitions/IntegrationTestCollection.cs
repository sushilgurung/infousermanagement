using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.IntegrationTesting.Fixtures;

namespace UserManagement.IntegrationTesting.CollectionDefinitions;
[CollectionDefinition("IntegrationTestCollection", DisableParallelization = true)]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestWebAppFactory> { }