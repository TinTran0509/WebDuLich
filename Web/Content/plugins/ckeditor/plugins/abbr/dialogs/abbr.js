/**
 * Copyright (c) 2014-2016, CKSource - Frederico Knabben. All rights reserved.
 * Licensed under the terms of the MIT License (see LICENSE.md).
 *
 * The abbr plugin dialog window definition.
 *
 * Created out of the CKEditor Plugin SDK:
 * http://docs.ckeditor.com/#!/guide/plugin_sdk_sample_1
 */

// Our dialog definition.
CKEDITOR.dialog.add( 'abbrDialog', function( editor ) {
	return {

		// Basic properties of the dialog window: title, minimum size.
		title: 'BIÊN TẬP NỘI DUNG TIN KHUYẾN MẠI',
		minWidth: 800,
		minHeight: 500,

		//// Dialog window content definition.
		contents: [
			{
				// Definition of the Basic Settings dialog tab (page).
				id: 'tab-basic',
				label: 'Basic Settings',

				// The tab content.
				elements: [
					{
						// Text input field for the abbreviation text.
					    type: 'textarea',
						id: 'abbr',
						label: 'NỘI DUNG BIÊN TẬP',
                        rows: 32,
						// Validation checking whether the field is not empty.
						validate: CKEDITOR.dialog.validate.notEmpty( "Bạn không được bỏ trống nội dung" )
					},
					//{
					//	// Text input field for the abbreviation title (explanation).
					//	type: 'text',
					//	id: 'title',
					//	label: 'Explanation',
					//	validate: CKEDITOR.dialog.validate.notEmpty( "Explanation field cannot be empty." )
					//}
				]
			},

			// Definition of the Advanced Settings dialog tab (page).
			
	    ],

		//onLoad: function () {
		//    // Register styles for placeholder widget frame.
		//    //var dialog = this;

		//    // Create a new <abbr> element.
		//    //var abbr = editor.document.createElement('div');

		//    //// Set element attribute and text by getting the defined field values.
		//    ////abbr.setAttribute( 'title', dialog.getValueOf( 'tab-basic', 'title' ) );
		//    //abbr.setHtml('<div id="slidePromotion"></div>');

		//    //// Now get yet another field value from the Advanced Settings tab.

		//    //// Finally, insert the element into the editor at the caret position.
		//    //editor.insertElement(abbr);

		//    alert(1);
		//},
		init: function(editor) {
		    alert(1);
		}

	    //// This method is invoked once a user clicks the OK button, confirming the dialog.
		//onOk: function() {

		//	// The context of this function is the dialog object itself.
		//	// http://docs.ckeditor.com/#!/api/CKEDITOR.dialog
		//	var dialog = this;

		//	// Create a new <abbr> element.
		//	var abbr = editor.document.createElement( 'div' );

		//	// Set element attribute and text by getting the defined field values.
		//	//abbr.setAttribute( 'title', dialog.getValueOf( 'tab-basic', 'title' ) );
		//	abbr.setHtml( '<div id="slidePromotion"></div>' ) ;

		//	// Now get yet another field value from the Advanced Settings tab.
			
		//	// Finally, insert the element into the editor at the caret position.
		//	editor.insertElement( abbr );
		//}
	};
});
