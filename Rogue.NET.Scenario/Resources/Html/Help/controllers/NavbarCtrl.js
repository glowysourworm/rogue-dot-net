function NavbarCtrl($scope, $location, $timeout){

	$scope.location = $location;

	$scope.$watch('location.$$absUrl', function () {
		$timeout(function () {
			set();
		});
	});
	
	function set() {
		$scope.intro = $location.absUrl().search('intro') > -1;
		$scope.combat = $location.absUrl().search('combat') > -1;
		$scope.skills = $location.absUrl().search('skills') > -1;
		$scope.community = $location.absUrl().search('community') > -1;
		$scope.navigation = $location.absUrl().search('navigation') > -1;
	}

	set();
}