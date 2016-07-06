angular.module('services', []);

angular.module('rogueHelp', [
		'ngRoute',
		'services'
	]).config(function ($routeProvider){

		$routeProvider.
			when('/home', {controller:HomeCtrl, templateUrl:'views/home.html'}).
			when('/intro', {controller:IntroCtrl, templateUrl:'views/intro.html'}).
			when('/combat', {controller:CombatCtrl, templateUrl:'views/combat.html'}).
			when('/community', {controller:CommunityCtrl, templateUrl:'views/community.html'}).
			when('/skills', {controller:IntroCtrl, templateUrl:'views/skills.html'}).
			when('/navigation', {controller:IntroCtrl, templateUrl:'views/navigation.html'}).
			otherwise({redirectTo:'/home'});
	});