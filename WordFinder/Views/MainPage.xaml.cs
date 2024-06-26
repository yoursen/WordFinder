﻿using WordFinder.Models;
using WordFinder.Services;
using WordFinder.ViewModels;

namespace WordFinder.Views;

public partial class MainPage : ContentPage
{
	private MainPageViewModel _viewModel;
	private GameDatabase _db;
	private TouchFeedbackService _feedback;
	public MainPage(MainPageViewModel viewModel, GameDatabase db, TouchFeedbackService feedback)
	{
		InitializeComponent();
		_viewModel = viewModel;
		_db = db;
		_feedback = feedback;
		BindingContext = viewModel;
	}

	private void OnPlayGameClicked(object sender, EventArgs args)
	{
		_feedback.Perform();
		_viewModel.PlayGameCommmand.Execute(null);
	}

	private void OnBestScoreClicked(object sender, EventArgs args)
	{
		_feedback.Perform();
		_viewModel.BestScoreCommmand.Execute(null);
	}
	private void OnSettingsClicked(object sender, EventArgs args)
	{
		_feedback.Perform();
		_viewModel.GameSettingsCommmand.Execute(null);
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
		await _db.Init();
    }
}

