This is to explain some of the definitions for this namespace

Processing:  
	
	Action - An Action is defined to be anything that sits on the scenario queues that is processed
			 by the ScenarioService (primary backend processor)

	Command - An Event Aggregator issuance that is directed AT the Backend, Frontend, Dialog, or View 
			  components

	Event - An Event Aggregator issuance that is bubbled up FROM the Backend, Frontend, Dialog, or View
			components

Game Router: This is the primary component to route in-game user commands and to store / manage state
			 that is intermediate to the various components that issue / process these commands.

Components:  [ Backend, Frontend, Dialog, View ]

	Backend - This is the set of services that operate directly on the Scenario model

	Frontend - This is the set of UI components that display data from the Scenario model

	Dialog - This is a view component that helps to process a sequence of events that 
		     involves the Backend / Frontend directly [ Identify, Uncurse, etc... ]; or
			 to show static views to the user like [ Help, Commands, Objectives, etc... ].

	View - This would be the view components that contain the primary game UI. Some commands
		   from the keyboard should be routed there. [ Show player sub-panel "Consumables", etc...]

Definitions:  Need a set of sub-definitions for the event aggregator that deal with these
		      components.

	GameCommand:  This is issued TO one of the four components ("Game" Components (?))

	GameEvent:    This is issued FROM one fo the four components

	GameCommand.Data / GameEvent.Data:  These store state for the above two issuances.