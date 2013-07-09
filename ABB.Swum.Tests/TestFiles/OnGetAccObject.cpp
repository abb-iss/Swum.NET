LRESULT CMenuContainer::OnGetAccObject( UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled ) {
    if ((DWORD)lParam==(DWORD)OBJID_CLIENT && m_pAccessible) {
        return LresultFromObject(IID_IAccessible,wParam,m_pAccessible);
    } else {
        bHandled=FALSE;
        return 0;
    }
}
