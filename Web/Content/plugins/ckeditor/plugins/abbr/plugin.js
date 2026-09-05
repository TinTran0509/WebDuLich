/**
 * Copyright (c) 2014-2016, CKSource - Frederico Knabben. All rights reserved.
 * Licensed under the terms of the MIT License (see LICENSE.md).
 *
 * Basic sample plugin inserting abbreviation elements into the CKEditor editing area.
 *
 * Created out of the CKEditor Plugin SDK:
 * http://docs.ckeditor.com/#!/guide/plugin_sdk_sample_1
 */

// Register the plugin within the editor.
CKEDITOR.plugins.add( 'abbr', {
	icons: 'abbr',
	init: function( editor ) {
	    editor.addCommand('abbr', {
	        exec: function (editor) {
	            editor.insertHtml('<div class="slidePromotion">SlidePromotion</div><br>');
	        }
	    });
		editor.ui.addButton( 'Abbr', {
		    label: 'BIÊN TẬP NỘI DUNG TIN KHUYẾN MẠI',
			command: 'abbr',
			toolbar: 'insert'
		});
	}
});
