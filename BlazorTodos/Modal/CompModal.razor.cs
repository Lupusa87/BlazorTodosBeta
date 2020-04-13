using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorTodos.Classes.Enums;

namespace BlazorTodos.Modal
{
    public partial class CompModal: IDisposable
    {

        [Parameter]
        public BTModal btModal { get; set; }


        protected string Title { get; set; }

        protected override void OnInitialized()
        {

            btModal.OnShow = ShowModal;

            btModal.OnClose = CloseModal;

            btModal.OnRefresh = Refresh;

            base.OnInitialized();
        }


        protected override void OnAfterRender(bool firstRender)
        {
            if (btModal.OnShow == null)
            {
                btModal.OnShow = ShowModal;
            }

            if (btModal.OnClose == null)
            {
                btModal.OnClose = CloseModal;
            }

            if (btModal.OnRefresh == null)
            {
                btModal.OnRefresh = Refresh;
            }




            //switch (btModal.modalForm)
            //{
            //    case ModalForm.Message:
            //        //LocalData.compMessage.refresh();
            //        break;
            //    case ModalForm.AddOrUpdateTodo:
            //        //LocalData.compAddOrUpdateTodoBase.Refresh();
            //        break;
            //    default:
            //        break;
            //}

            base.OnAfterRender(firstRender);
        }



        public void Refresh()
        {
            StateHasChanged();
        }


        public void ShowModal()
        {

            switch (btModal.modalForm)
            {
                case ModalForm.Message:
                    Title = "Message";
                  
                    if (LocalData.BTMessagesList.Any())
                    {
                    
                        if (LocalData.BTMessagesList.Count == 1)
                        {
                         
                            Title = "Message";
                        }
                        else
                        {
                          
                            Title = "Messages (" + LocalData.BTMessagesList.Count + ")";
                        }
                    }
                    break;
                case ModalForm.Error:
                    Title = "Error";
                    if (LocalData.BTErrorsList.Any())
                    {
                        if (LocalData.BTErrorsList.Count == 1)
                        {
                            Title = "Error";
                        }
                        else
                        {
                            Title = "Errors ("+ LocalData.BTErrorsList.Count + ")";
                        }


                        if (!btModal.CanCloseModal)
                        {
                            Title = "Fatal " + Title;
                        }
                    }
                    break;
                case ModalForm.Confirm:
                    Title = "Warning";
                    break;
                case ModalForm.Login:
                    Title = "Login";
                    break;
                case ModalForm.Registration:
                    Title = "Registration";
                    break;
                case ModalForm.DefaultFont:
                    Title = "Set default font";
                    break;
                case ModalForm.AddOrUpdateTodo:
                    if (LocalData.AddOrUpdateMode)
                    {
                        Title = "Add todo";
                    }
                    else
                    {
                        Title = "Update todo";
                    }
                    break;
                case ModalForm.AddOrUpdateCategory:
                    if (LocalData.AddOrUpdateMode)
                    {
                        Title = "Add category";
                    }
                    else
                    {
                        Title = "Update category";
                    }
                    break;
                case ModalForm.AddOrUpdateFeedback:
                    if (LocalData.AddOrUpdateMode)
                    {
                        Title = "Add feedback";
                    }
                    else
                    {
                        Title = "Update feedback";
                    }
                    break;
                case ModalForm.ChangePassword:
                    Title = "Change Password";
                    break;
                default:
                    break;
            }


            btModal.IsDisplayed = true;

            StateHasChanged();
        }

        public void CloseModal()
        {

            if (btModal.CanCloseModal)
            {
                Title = string.Empty;
                btModal.IsDisplayed = false;

                switch (btModal.modalForm)
                {
                    case ModalForm.Message:
                        LocalFunctions.ClearMessages();
                        break;
                    case ModalForm.Error:
                        LocalFunctions.ClearErrors();
                        break;
                    default:
                        break;
                }

                StateHasChanged();
            }
            
        }

        public void Dispose()
        {

        }

      
    }
}
